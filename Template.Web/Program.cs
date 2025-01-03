using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.Net.Http.Headers;

using PdfSharp.Fonts;

using Prometheus;

using Rester;

using Serilog;

using Smart.AspNetCore;
using Smart.AspNetCore.ApplicationModels;
using Smart.Data;
using Smart.Data.Accessor.Extensions.DependencyInjection;
using Smart.Data.SqlClient;

using StackExchange.Profiling;
using StackExchange.Profiling.Data;

using Template.Components.Report;
using Template.Components.Security;
using Template.Components.Storage;
using Template.Web;
using Template.Web.Application.HealthChecks;
using Template.Web.Application.RateLimiting;

#pragma warning disable CA1812

//--------------------------------------------------------------------------------
// Configure builder
//--------------------------------------------------------------------------------
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
});

// Service
builder.Host
    .UseWindowsService()
    .UseSystemd();

// Configuration
var serverSetting = builder.Configuration.GetSection("Server").Get<ServerSetting>()!;
builder.Services.AddSingleton(serverSetting);

// Log
builder.Logging.ClearProviders();
builder.Services.AddSerilog(option =>
{
    option.ReadFrom.Configuration(builder.Configuration);
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpLogging(static options =>
    {
        //options.LoggingFields = HttpLoggingFields.All | HttpLoggingFields.RequestQuery;
        options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders |
                                HttpLoggingFields.RequestQuery |
                                HttpLoggingFields.ResponsePropertiesAndHeaders;
    });
}

// System
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// PDF
GlobalFontSettings.FontResolver = new FontResolver(Directory.GetCurrentDirectory(), FontNames.Gothic, new Dictionary<string, string>
{
    { FontNames.Gothic, "ipaexg.ttf" }
});

// Add framework Services.
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

// Feature management
builder.Services.AddFeatureManagement();

// Size limit
builder.Services.Configure<FormOptions>(static options =>
{
    // Default 4MB
    options.ValueLengthLimit = int.MaxValue;
    // Default 128MB
    options.MultipartBodyLengthLimit = Int64.MaxValue;
});

// Route
builder.Services.Configure<RouteOptions>(static options =>
{
    options.AppendTrailingSlash = true;
});

// XForward
builder.Services.Configure<ForwardedHeadersOptions>(static options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    // Do not restrict to local network/proxy
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// CORS
//builder.Services.Configure<CorsOptions>(static options =>
//{
//});

// Filter
builder.Services.AddTimeLogging(options =>
{
    options.Threshold = serverSetting.LongTimeThreshold;
});

// Mvc
builder.Services
    .AddControllersWithViews(static options =>
    {
        options.Conventions.Add(new LowercaseControllerModelConvention());
        options.Filters.AddTimeLogging();
    })
#if DEBUG
    .AddRazorRuntimeCompilation()
#endif
    .AddJsonOptions(static options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        options.JsonSerializerOptions.Converters.Add(new Template.Components.Json.DateTimeConverter());
    });

builder.Services.AddEndpointsApiExplorer();

// Swagger
if (!builder.Environment.IsProduction())
{
    builder.Services.AddSwaggerGen();
}

// Rate limit
builder.Services.AddRateLimiter(builder.Configuration.GetSection("RateLimit").Get<RateLimitSetting>()!);

// Error handler
builder.Services.AddProblemDetails(static options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.Add("nodeId", Environment.MachineName);
    };
});

// SignalR
builder.Services.AddSignalR();

// Compress
//builder.Services.AddRequestDecompression();
//builder.Services.AddResponseCompression(static options =>
//{
//    // Default false (for CRIME and BREACH attacks)
//    options.EnableForHttps = true;
//    options.Providers.Add<GzipCompressionProvider>();
//    options.MimeTypes = new[] { MediaTypeNames.Application.Json };
//});
//builder.Services.Configure<GzipCompressionProviderOptions>(static options =>
//{
//    options.Level = CompressionLevel.Fastest;
//});

// Health
builder.Services
    .AddHealthChecks()
    .AddCheck<CustomHealthCheck>("custom_check", tags: ["app"]);

// Develop
if (!builder.Environment.IsProduction())
{
    // Profiler
    builder.Services.AddMiniProfiler(static options =>
    {
        options.RouteBasePath = "/profiler";
    });
}

// Authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(static options =>
    {
        options.Cookie.Name = "__account";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(1440);
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;

        options.LoginPath = new PathString("/account/login");
        options.LogoutPath = new PathString("/account/logout");
        options.AccessDeniedPath = new PathString("/error/403");
    });

builder.Services.AddSingleton<AccountManager>();

// Mapper
builder.Services.AddSingleton<IMapper>(new Mapper(new MapperConfiguration(c =>
{
    c.AddProfile<Template.Web.Areas.Default.MappingProfile>();
    c.AddProfile<Template.Web.Areas.Admin.MappingProfile>();
    c.AddProfile<Template.Web.Areas.Api.MappingProfile>();
    c.AddProfile<Template.Web.Areas.Example.MappingProfile>();
})));

// HttpClient
var connectorSetting = builder.Configuration.GetSection("Connector").Get<ConnectorSetting>()!;
builder.Services.AddHttpClient(ConnectorNames.Sample, c =>
{
    c.BaseAddress = new Uri(connectorSetting.SampleBaseUrl);
    // [MEMO] Set headers here
});

RestConfig.Default.UseJsonSerializer(config =>
{
    config.Converters.Add(new Template.Components.Json.DateTimeConverter());
    config.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    config.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Database
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddSingleton<IDbProvider>(builder.Environment.IsProduction()
    ? new DelegateDbProvider(() => new SqlConnection(connectionString))
    : new DelegateDbProvider(() => new ProfiledDbConnection(new SqlConnection(connectionString), MiniProfiler.Current)));

builder.Services.AddSingleton<IDialect, SqlDialect>();

builder.Services.AddDataAccessor(c =>
{
    c.EngineOption.ConfigureTypeMap(map =>
    {
        map[typeof(DateTime)] = DbType.DateTime2;
    });
});

// Security
builder.Services.AddSingleton<DefaultPasswordProviderOptions>();
builder.Services.AddSingleton<IPasswordProvider, DefaultPasswordProvider>();

// Storage
builder.Services.Configure<FileStorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.AddSingleton(p => p.GetRequiredService<IOptions<FileStorageOptions>>().Value);
builder.Services.AddSingleton<IStorage, FileStorage>();

// Service
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<DataService>();
builder.Services.AddSingleton<ItemService>();
builder.Services.AddSingleton<ConnectorService>();

// Csv
// TODO
//builder.Services.AddSingleton<CsvImporter>();
//builder.Services.AddSingleton<CsvExporter>();

// Report
// TODO
//builder.Services.AddSingleton<ExampleReportBuilder>();

// Hub
// TODO

//--------------------------------------------------------------------------------
// Configure the HTTP request pipeline
//--------------------------------------------------------------------------------

var app = builder.Build();

// Startup information
ThreadPool.GetMinThreads(out var workerThreads, out var completionPortThreads);
app.Logger.InfoServiceStart();
app.Logger.InfoServiceSettingsEnvironment(typeof(Program).Assembly.GetName().Version, Environment.Version, Environment.CurrentDirectory);
app.Logger.InfoServiceSettingsGC(GCSettings.IsServerGC, GCSettings.LatencyMode, GCSettings.LargeObjectHeapCompactionMode);
app.Logger.InfoServiceSettingsThreadPool(workerThreads, completionPortThreads);

// Log
if (app.Environment.IsDevelopment())
{
    // Serilog
    app.UseSerilogRequestLogging(static options =>
    {
        options.IncludeQueryInRequestPath = true;
    });

    // HTTP log
    app.UseWhen(
        static c => c.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase),
        static b => b.UseHttpLogging());
    //app.UseHttpLogging();
}

// Forwarded headers
app.UseForwardedHeaders();

// Error handler
if (!app.Environment.IsDevelopment())
{
    app.UseWhen(
        static c => c.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase),
        static b => b.UseExceptionHandler(),
        static b =>
        {
            b.UseExceptionHandler("/Home/Error");
            b.UseStatusCodePagesWithReExecute("/error/{0}");
        });
}
else
{
    app.UseWhen(
        static c => c.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase),
        static b => b.UseExceptionHandler(),
        static b => b.UseStatusCodePagesWithReExecute("/error/{0}"));
}

// Develop
if (!app.Environment.IsProduction())
{
    // Profiler
    app.UseMiniProfiler();

    // Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HSTS
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// HTTPS redirection
app.UseHttpsRedirection();

// Static files
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public, max-age=31536000";
        ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddYears(1).ToString("R", CultureInfo.InvariantCulture));
    }
});

// Coolie policy
//app.UseCookiePolicy();

// Routing
app.UseRouting();

// Rate limit
app.UseWhen(
    static c => c.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase),
    static b => b.UseRateLimiter());

// Localize
//app.UseRequestLocalization();

// CORS
//app.UseCors();

// Metrics
app.UseHttpMetrics();

// Authentication/Authorization
app.UseAuthentication();
app.UseAuthorization();

// Session
//app.UseSession();

// Compress
//app.UseWhen(
//    static c => c.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase),
//    static b =>
//    {
//        b.UseResponseCompression();
//        b.UseRequestDecompression();
//    });

// Cache
// app.UseResponseCaching();

// Map
app.MapControllers();

// TODO SignalR

// Metrics
app.MapMetrics();

// Health
app.MapHealthChecks("/health");

// Initialize
await app.InitializeAsync();

// Run
await app.RunAsync();

// For test
public sealed partial class Program
{
}

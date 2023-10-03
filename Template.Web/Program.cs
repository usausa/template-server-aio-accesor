using System.IO.Compression;
using System.Net.Mime;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.Net.Http.Headers;

using PdfSharpCore.Fonts;

using Prometheus;

using Rester;

using Serilog;

using Smart.AspNetCore;
using Smart.AspNetCore.ApplicationModels;
using Smart.AspNetCore.Filters;
using Smart.Data;
using Smart.Data.Accessor.Extensions.DependencyInjection;
using Smart.Data.SqlClient;

using StackExchange.Profiling;
using StackExchange.Profiling.Data;

using Template.Components.Report;
using Template.Components.Security;
using Template.Components.Storage;
using Template.Web.Application.HealthChecks;

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

// Log
builder.Logging.ClearProviders();
builder.Services.AddSerilog(option =>
{
    option.ReadFrom.Configuration(builder.Configuration);
});

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

// Settings
var serverSetting = builder.Configuration.GetSection("Server").Get<ServerSetting>()!;
builder.Services.AddSingleton(serverSetting);

// Feature management
builder.Services.AddFeatureManagement();

// Route
builder.Services.Configure<RouteOptions>(options =>
{
    options.AppendTrailingSlash = true;
});

// Size limit
builder.Services.Configure<FormOptions>(options =>
{
    // Default 4MB
    options.ValueLengthLimit = int.MaxValue;
    // Default 128MB
    options.MultipartBodyLengthLimit = Int64.MaxValue;
});

// XForward
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    // Do not restrict to local network/proxy
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// CORS
//builder.Services.Configure<CorsOptions>(options =>
//{
//});

// Filter
builder.Services.AddSingleton<ExceptionStatusFilter>();
builder.Services.AddTimeLogging(options =>
{
    options.Threshold = serverSetting.LongTimeThreshold;
});
builder.Services.AddSingleton(new TokenSetting { Token = serverSetting.ApiToken });

// Mvc
builder.Services
    .AddControllersWithViews(options =>
    {
        options.Filters.AddExceptionStatus();
        options.Filters.AddTimeLogging();
        options.Conventions.Add(new LowercaseControllerModelConvention());
    })
#if DEBUG
    .AddRazorRuntimeCompilation()
#endif
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        options.JsonSerializerOptions.Converters.Add(new Template.Components.Json.DateTimeConverter());
    });

// SignalR
builder.Services.AddSignalR();

// Compress
builder.Services.AddRequestDecompression();
builder.Services.AddResponseCompression(options =>
{
    // Default false (for CRIME and BREACH attacks)
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = new[] { MediaTypeNames.Application.Json };
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.AddEndpointsApiExplorer();

// Authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
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

// Health
builder.Services
    .AddHealthChecks()
    .AddCheck<CustomHealthCheck>("custom_check", tags: new[] { "app" });

// Develop
if (!builder.Environment.IsProduction())
{
    // Swagger
    builder.Services.AddSwaggerGen();

    // Profiler
    builder.Services.AddMiniProfiler(options =>
    {
        options.RouteBasePath = "/profiler";
    });
}

//--------------------------------------------------------------------------------
// Configure the HTTP request pipeline
//--------------------------------------------------------------------------------

var app = builder.Build();

// Serilog
if (!app.Environment.IsProduction())
{
    app.UseSerilogRequestLogging(options =>
    {
        options.IncludeQueryInRequestPath = true;
    });
}

// Error handler
if (!app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error/500");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/error/{0}");

// Forwarded headers
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
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

// Health
app.UseHealthChecks("/health");

// Metrics
app.UseHttpMetrics();

// Develop
if (!app.Environment.IsProduction())
{
    // Profiler
    app.UseMiniProfiler();

    // Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Routing
app.UseRouting();

// Compress
app.UseResponseCompression();
app.UseRequestDecompression();

// CORS
//app.UseCors();

// Authentication/Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map
app.MapControllers();
// TODO SignalR

// Metrics
app.MapMetrics();

// Run
app.Run();

// For test
#pragma warning disable CA1050
public partial class Program
{
}

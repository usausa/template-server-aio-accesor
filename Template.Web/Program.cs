using System.Data;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;

using AspNetCoreComponents.IpFilter;

using AutoMapper;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
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

using Template.Components.Json;
using Template.Components.Report;
using Template.Components.Security;
using Template.Components.Storage;
using Template.Services;
using Template.Web.Authentication;
using Template.Web.Infrastructure.Token;
using Template.Web.Settings;

#pragma warning disable CA1812

//--------------------------------------------------------------------------------
// Configure builder
//--------------------------------------------------------------------------------

var builder = WebApplication.CreateBuilder(args);

// Log
builder.Host
    .ConfigureLogging((_, logging) =>
    {
        logging.ClearProviders();
    })
    .UseSerilog((hostingContext, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
    });

// System
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// PDF
GlobalFontSettings.FontResolver = new FontResolver(Directory.GetCurrentDirectory(), FontNames.Gothic, new Dictionary<string, string>
{
    { FontNames.Gothic, "ipaexg.ttf" }
});

// Add framework builder.Services.
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

// Settings
var serverSetting = builder.Configuration.GetSection("Server").Get<ServerSetting>();
builder.Services.AddSingleton(serverSetting);

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
        options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    });

// SignalR
builder.Services.AddSignalR();

// Compress
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
    options.EnableForHttps = true;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Profiler
if (!builder.Environment.IsProduction())
{
    builder.Services.AddMiniProfiler(options =>
    {
        options.RouteBasePath = "/profiler";
    });
}

// Health
builder.Services.AddHealthChecks();

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
var connectorSetting = builder.Configuration.GetSection("Connector").Get<ConnectorSetting>();
builder.Services.AddHttpClient(ConnectorNames.Sample, c =>
{
    c.BaseAddress = new Uri(connectorSetting.SampleBaseUrl);
    // [MEMO] Set headers here
});

RestConfig.Default.UseJsonSerializer(config =>
{
    config.Converters.Add(new DateTimeConverter());
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
builder.Services.AddSingleton<SaltHashPasswordOptions>();
builder.Services.AddSingleton<IPasswordProvider, SaltHashPasswordProvider>();

// Storage
builder.Services.Configure<FileStorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.AddSingleton(p => p.GetService<IOptions<FileStorageOptions>>()!.Value);
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
app.UsePathRestrict("/health", serverSetting.AllowHealth?.Select(System.Net.IPNetwork.Parse).ToArray());
app.UseHealthChecks("/health");

// Metrics
app.UsePathRestrict("/metrics", serverSetting.AllowMetrics?.Select(System.Net.IPNetwork.Parse).ToArray());
app.UseHttpMetrics();

// Profiler
if (!app.Environment.IsProduction())
{
    app.UseMiniProfiler();
}

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Compress
app.UseResponseCompression();
app.UseRequestDecompress();

// Routing
app.UseRouting();

// CORS
//app.UseCors();

// Authentication/Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map
app.MapControllers();
// TODO SignalR
app.MapMetrics();

app.Run();

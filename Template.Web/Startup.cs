namespace Template.Web
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Text.Unicode;

    using AutoMapper;

    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Net.Http.Headers;
    using Microsoft.OpenApi.Models;

    using Smart.AspNetCore;
    using Smart.AspNetCore.ApplicationModels;
    using Smart.AspNetCore.Filters;
    using Smart.Data;
    using Smart.Data.Accessor.Extensions.DependencyInjection;
    using Smart.Data.SqlClient;

    using StackExchange.Profiling;
    using StackExchange.Profiling.Data;

    using Template.Web.Authentication;
    using Template.Web.Settings;

    public class Startup
    {
        private readonly IWebHostEnvironment env;

        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            this.env = env;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // System
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // PDF
            // TODO
            // ReSharper disable once CommentTypo
            //GlobalFontSettings.FontResolver = new FontResolver(Directory.GetCurrentDirectory(), FontNames.Gothic, new Dictionary<string, string>
            //{
            //    { FontNames.Gothic, "ipaexg.ttf" }
            //});

            // Add framework services.
            services.AddHttpContextAccessor();
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            // Settings
            var serverSetting = Configuration.GetSection("Server").Get<ServerSetting>();

            // Mvc
            services.AddSingleton<ExceptionStatusFilter>();
            services.AddTimeLogging(options =>
            {
                options.Threshold = serverSetting.LongTimeThreshold;
            });

            services.Configure<RouteOptions>(options =>
            {
                options.AppendTrailingSlash = true;
            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = Int64.MaxValue;
            });

            services
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
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                    // TODO
                    //options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                });

            // Compress
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                // json only
                options.MimeTypes = new[] { "application/json" };
            });

            // SignalR
            services.AddSignalR();

            // Swagger
            if (!env.IsProduction())
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("template", new OpenApiInfo { Title = "Template API", Version = "v1" });
                    // TODO
                    //options.OperationFilter<TokenOperationFilter>(serverSetting.ApiToken);
                });
            }

            // Profiler
            if (!env.IsProduction())
            {
                services.AddMiniProfiler(options =>
                {
                    options.RouteBasePath = "/profiler";
                });
            }

            // Health
            services.AddHealthChecks();

            // Authentication
            services
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

            services.AddSingleton<AccountManager>();

            // Mapper
            services.AddSingleton<IMapper>(new Mapper(new MapperConfiguration(c =>
            {
                c.AddProfile<Template.Web.Areas.Default.MappingProfile>();
                // TODO
                //c.AddProfile<Template.Web.Areas.Api.MappingProfile>();
            })));

            // Http

            // Database
            var connectionString = Configuration.GetConnectionString("Default");
            services.AddSingleton<IDbProvider>(env.IsProduction()
                ? new DelegateDbProvider(() => new SqlConnection(connectionString))
                : new DelegateDbProvider(() => new ProfiledDbConnection(new SqlConnection(connectionString), MiniProfiler.Current)));

            services.AddSingleton<IDialect, SqlDialect>();

            services.AddDataAccessor(c =>
            {
                c.EngineOption.ConfigureTypeMap(map =>
                {
                    map[typeof(DateTime)] = DbType.DateTime2;
                });
            });

            // Security
            // TODO
            //services.AddSingleton<SaltHashPasswordOptions>();
            //services.AddSingleton<IPasswordProvider, SaltHashPasswordProvider>();

            // Csv
            // TODO
            //services.AddSingleton<CsvImporter>();
            //services.AddSingleton<CsvExporter>();

            // Context
            // TODO
            //services.AddSingleton<IServiceContext>(new ServiceContext(serverSetting.SystemName));

            // Service
            // TODO

            // Hub
            // TODO

            // Report
            // TODO
        }

        public void Configure(IApplicationBuilder app)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/500");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseHealthChecks("/health");

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public, max-age=31536000";
                    ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddYears(1).ToString("R", CultureInfo.InvariantCulture));
                }
            });

            if (!env.IsProduction())
            {
                app.UseMiniProfiler();
            }

            app.UseResponseCompression();
            app.UseRequestDecompress();

            // Swagger
            if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/template/swagger.json", "TMP API");
                });
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapHub<SummaryHub>("/hub/summary");
            });
        }
    }
}

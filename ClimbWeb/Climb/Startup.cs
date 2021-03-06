using System.Linq;
using System.Threading.Tasks;
using Climb.Core.TieBreakers;
using Climb.Data;
using Climb.Models;
using Climb.Services;
using Climb.Services.HealthChecks;
using Climb.Services.ModelServices;
using Climb.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Climb
{
    public class Startup
    {
        private readonly ILogger<Startup> logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            this.logger = logger;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var healthChecks = services.AddHealthChecks();

            ConfigureDB(services, healthChecks);
            ConfigureIdentity(services);

            services.AddAuthentication();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddCookieTempDataProvider();

            ConfigureCdn(services);
            AddTransient(services);
        }

        private void AddTransient(IServiceCollection services)
        {
            services.AddTransient<IApplicationUserService, ApplicationUserService>();
            services.AddTransient<IGameService, GameService>();
            services.AddTransient<ILeagueService, LeagueService>();
            services.AddTransient<ISeasonService, SeasonService>();
            services.AddTransient<ISetService, SetService>();
            services.AddTransient<IOrganizationService, OrganizationService>();
            services.AddTransient<ITokenHelper, TokenHelper>();
            services.AddTransient<IUrlUtility, UrlUtility>();
            services.AddTransient<IScheduleFactory, RoundRobinScheduler>();
            services.AddTransient<IPointService, EloPointService>();
            services.AddTransient<ISeasonPointCalculator, ParticipationSeasonPointCalculator>();
            services.AddTransient<ITieBreakerFactory, TieBreakerFactory>();
            services.AddTransient<ISignInManager, SignInManager>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<IAnalyzerService, AnalyzerService>();
            services.AddTransient<IAnalyzerFactory, AnalyzerFactory>();

            if(string.IsNullOrWhiteSpace(Configuration[ControlledDateService.OverrideKey]))
            {
                services.AddTransient<IDateService, DateService>();
            }
            else
            {
                services.AddTransient<IDateService, ControlledDateService>();
            }

            if(string.IsNullOrWhiteSpace(Configuration["Email:Key"]))
            {
                services.AddTransient<IEmailSender, NullEmailService>();
            }
            else
            {
                services.AddTransient<IEmailSender, SendGridService>();
            }

            services.AddSwaggerDocument();
        }

        private static void ConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

        private void ConfigureCdn(IServiceCollection services)
        {
            var cdnType = Configuration["CDN"];
            switch(cdnType)
            {
                case "S3":
                    services.AddSingleton<ICdnService, S3Cdn>();
                    break;
                case "Local":
                    services.AddSingleton<ICdnService, FileStorageCdn>();
                    break;
                default:
                    logger.LogWarning("No CDN established");
                    break;
            }
        }

        private void ConfigureDB(IServiceCollection services, IHealthChecksBuilder healthChecks)
        {
            using(logger.BeginScope("Configuring DB"))
            {
                var connectionString = Configuration.GetConnectionString("defaultConnection");
                if(string.IsNullOrWhiteSpace(connectionString))
                {
                    logger.LogInformation("Using In Memory DB");
                    services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("Test"));
                }
                else
                {
                    logger.LogInformation("Using SQL Server DB");
                    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
                }

                healthChecks.AddCheck("Database", new SqlConnectionHealthCheck(connectionString));
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Site/Error", "?statusCode={0}");
            app.UseExceptionHandler("/Site/Error");
            app.UseHealthChecks("/health", new HealthCheckOptions {ResponseWriter = WriteResponse});
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUi3();

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }

        private static Task WriteResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(
                            p => new JProperty(p.Key, p.Value))))))))));
            return httpContext.Response.WriteAsync(
                json.ToString(Formatting.Indented));
        }
    }
}
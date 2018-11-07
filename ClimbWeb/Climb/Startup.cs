using System.Reflection;
using Climb.Core.TieBreakers;
using Climb.Data;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NJsonSchema;
using NSwag.AspNetCore;

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
            ConfigureDB(services);

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

            services.AddAuthentication();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddCookieTempDataProvider();

            ConfigureCdn(services);
            ConfigureTransient(services);

            if (string.IsNullOrWhiteSpace(Configuration[ControlledDateService.OverrideKey]))
            {
                services.AddTransient<IDateService, DateService>();
            }
            else
            {
                services.AddTransient<IDateService, ControlledDateService>();
            }

            if (string.IsNullOrWhiteSpace(Configuration["Email:Key"]))
            {
                services.AddTransient<IEmailSender, NullEmailService>();
            }
            else
            {
                services.AddTransient<IEmailSender, SendGridService>();
            }
        }

        private static void ConfigureTransient(IServiceCollection services)
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
            services.AddTransient<ITournamentService, TournamentService>();
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

        private void ConfigureDB(IServiceCollection services)
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
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                //{
                //    HotModuleReplacement = false,
                //    ReactHotModuleReplacement = false,
                //    EnvironmentVariables = new Dictionary<string, string> {{"mode", "development"}},
                //});
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Site/Error", "?statusCode={0}");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

#pragma warning disable 618
            // Need this to register middlewares until NSwag problem is fixed.
            app.UseSwaggerUi3(typeof(Startup).GetTypeInfo().Assembly, s => { });
#pragma warning restore 618
            app.UseSwaggerUi3WithApiExplorer(settings => settings.GeneratorSettings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase);

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}
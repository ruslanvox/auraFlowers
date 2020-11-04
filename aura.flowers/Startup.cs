using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Globalization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using website.core.Models.Auth;
using website.services.Extensions;
using website.core.Models.Email;
using website.core.Models.GoogleRecaptcha;
using website.data;

namespace aura.flowers
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Dependency injection:
            services.AddInjections();

            // Setup singleton and config classes by config file sections:
            services.AddSingleton(_ => Configuration);
            services.Configure<EmailConfiguration>(Configuration.GetSection("EmailConfiguration"));
            services.Configure<GoogleRecaptchaConfiguration>(Configuration.GetSection("GoogleRecaptchaConfiguration"));

            // Website context configuration:
            services.AddDbContext<WebsiteContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("WebsiteConnectionString")));

            // Authorization by Identity configuration (using Pomelo MySQL provider):
            services.AddDbContext<IdentityContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("IdentityConnectionString")));
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>();

            // Cookie configuration:
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });

            // Localization:
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            CultureInfo[] supportedCultures = {
                    new CultureInfo("en-US"),
                    new CultureInfo("ru-RU"),
                    //new CultureInfo("de-DE"),
                    new CultureInfo("sk-SK")
            };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.DefaultRequestCulture = new RequestCulture("en-US", "en-US");
            });

            // MVC initialization:
            services.AddControllersWithViews()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddDataAnnotationsLocalization()
                .AddViewLocalization()
                .AddRazorRuntimeCompilation();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
            }

            // Localization:
            IOptions<RequestLocalizationOptions> localizeOptions =
                app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizeOptions.Value);

            app.UseStaticFiles();
            //app.UseCookiePolicy();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseDefaultFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

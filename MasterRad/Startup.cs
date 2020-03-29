using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coravel;
using MasterRad.Extensions;
using MasterRad.Models;
using MasterRad.Models.Configuration;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using Microsoft.Identity.Web.UI;
using WebApp_OpenIDConnect_DotNet.Services;

namespace MasterRad
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
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.HandleSameSiteCookieCompatibility();
            });

            services.AddOptions();

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                    .AddSignIn("AzureAd", Configuration, options => Configuration.Bind("AzureAd", options));

            var initialScopes = new string[] { Constants.ScopeUserRead }; //scope consent's required for every authenthicated user
            services.AddWebAppCallsProtectedWebApi(Configuration, initialScopes)
                    .AddInMemoryTokenCaches();

            var sqlServerAdminConfigSection = Configuration.GetSection("SqlServerAdminConnection");
            var dbConfig = new ConnectionParams();
            sqlServerAdminConfigSection.Bind(dbConfig);
            services.AddDbContext<Context>(opts => opts.UseSqlServer(new SqlConnection(dbConfig.EFConnectionString)));

            //services
            services.AddScoped<IMicrosoftSQL, MicrosoftSQL>();
            services.AddSingleton<IMsSqlQueryBuilder, MsSqlQueryBuilder>();
            services.AddScoped<IUser, User>();
            services.AddScoped<IEvaluator, Evaluator>();
            services.AddScoped(typeof(ISignalR<>), typeof(SignalR<>));
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<ITokenAcquisition, TokenAcquisition>();

            //configurations
            services.AddGraphService(Configuration);
            services.Configure<SqlServerAdminConnection>(sqlServerAdminConfigSection);

            //repositories
            services.AddScoped<ITemplateRepository, TemplateRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ISynthesisRepository, SynthesisRepository>();
            services.AddScoped<IAnalysisRepository, AnalysisRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();

            services.AddControllersWithViews
                    (options =>
                    {
                        var policyBuilder = new AuthorizationPolicyBuilder();
                        var policy = policyBuilder.RequireAuthenticatedUser()
                                                  .Build();
                        options.Filters.Add(new AuthorizeFilter(policy));
                    })
                    .AddMicrosoftIdentityUI();

            services.AddRazorPages();

            services.AddQueue();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            app.AddCustomExceptionMiddleware();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<SynthesisProgressHub>("/synthesisprogress");
                endpoints.MapHub<AnalysisProgressHub>("/analysisprogress");
            });

            

            var appDataPath = $"{AppContext.BaseDirectory}\\AppData";
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);
        }
    }
}

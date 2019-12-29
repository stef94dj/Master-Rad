using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coravel;
using MasterRad.Extensions;
using MasterRad.Models;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var conf = new ConnectionParams();
            Configuration.Bind("DbAdminConnection", conf);
            var connString = $"server={conf.ServerName};database={conf.DbName};User ID={conf.Login};password={conf.Password};";
            services.AddDbContext<Context>(opts => opts.UseSqlServer(new SqlConnection(connString)));

            //services
            services.AddScoped<IMicrosoftSQL, MicrosoftSQL>();
            services.AddSingleton<IMsSqlQueryBuilder, MsSqlQueryBuilder>();
            services.AddScoped<IUser, User>();
            services.AddScoped<IEvaluator, Evaluator>();
            services.AddScoped(typeof(ISignalR<>), typeof(SignalR<>));
            services.AddScoped<ILogRepository, LogRepository>();

            //repositories
            services.AddScoped<ITemplateRepository, TemplateRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ISynthesisRepository, SynthesisRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddQueue();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.AddCustomExceptionMiddleware();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseSignalR(routes => { routes.MapHub<JobProgressHub>("/jobprogress"); });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            var appDataPath = $"{AppContext.BaseDirectory}\\AppData";
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.APIServer
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
            services.AddControllersWithViews();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


            string filepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\mydb.db";
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
            Thread.Sleep(100);
            IDFManager.Configure(new IDFManagerConfig
            {

                DatabaseType = DatabaseType.SQLite,
                DBConnectionString = "Data Source=mydb.db;",
                AdministratorUserName = "Admin",
                AdministratorPassword = "P@ssw0rd",
                AdministratorRoleId = "Administrators",
                AdministratorRoleName = "Administrators",
                DefaultListPageSize = 25,
                DBLifeCycle = DBLifeCycle.Both,
                IsActiveByDefault = true,
                IsLockedByDefault = false,
                DefaultTokenLifeTime = new LifeTime(30, 0, 0),
                AllowUserMultipleSessions = false,
                TokenEncryptionKey = "123456",
                TokenValidationMode = TokenValidationMode.DecryptOnly
            }); 


        }

       
    }
}

using BlazorUIDemo.Components;
using BlazorUIDemo.Models;
using CMouss.IdentityFramework;
using CMouss.IdentityFramework.BlazorUI;
using Microsoft.AspNetCore.Routing;

namespace BlazorUIDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<Components.App>()
                .AddInteractiveServerRenderMode()
                .AddAdditionalAssemblies(typeof(CMouss.IdentityFramework.BlazorUI.AssemblyMarker).Assembly);

            IDFManager.Configure(new IDFManagerConfig
            {
                DatabaseType = DatabaseType.SQLite,
                DBConnectionString = "Data Source=database.db;",
                DefaultListPageSize = 25,
                DBLifeCycle = DBLifeCycle.Both,
                IsActiveByDefault = true,
                IsLockedByDefault = false,
                DefaultTokenLifeTime = new LifeTime(365, 0, 0),
                AllowUserMultipleSessions = false,
                TokenEncryptionKey = "123456",
                AdministratorUserName = "admin",
                AdministratorPassword = "admin",
                AdministratorRoleName = "Administrators",
                TokenValidationMode = TokenValidationMode.DecryptOnly

                ,
                AuthenticationBackend = AuthenticationBackend.LDAP
                    ,
                AD_LDAP = "LDAP://10.38.38.71"
                    ,
                AD_Domain = "sme.gov.om"
                    ,
                AD_User = "12345678"
                    ,
                AD_Password = "12345678"
                    ,
                AD_UseSSL = false
                    ,
                AD_BaseDN = "DC=sme,DC=gov,DC=om"

            });
            DemoDBContext db = new();
            db.Database.EnsureCreated();
            db.InsertMasterData();
            IDFManager.RefreshIDFStorage();


            IDFBlazorUIConfig.AuthHomeURL = "/weather";



            app.Run();
        }
    }
}

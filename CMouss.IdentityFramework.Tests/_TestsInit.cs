using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.Tests
{
    [TestClass]
    public class SetupAssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            string filepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\mydb.db";
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
            Thread.Sleep(100);
            IDFManager.Configure(new IDFManagerConfig
            {

                //DatabaseType = DatabaseType.MSSQL,
                //DBConnectionString = "Server=NUC;Database=ProjectBravos2;User Id=sa;Password=SMedi@22222;",


                DatabaseType = DatabaseType.SQLite,
                DBConnectionString = "Data Source=mydb.db;",
                DefaultListPageSize = 25,
                DBLifeCycle = DBLifeCycle.Both,
                IsActiveByDefault = true,
                IsLockedByDefault = false
            }); ;

            CustomDBModel db = new CustomDBModel();
            db.Database.EnsureCreated();
            db.InsertMasterData();
            Thread.Sleep(100);
            CreateTestData();
            Thread.Sleep(100);
            IDFManager.RefreshDBContext_MasterData();
        }

        public static void CreateTestData()
        {

            User admin1 = new User();
            admin1.Id = Guid.NewGuid().ToString();
            admin1.UserName = "Admin";
            admin1.PrivateKey = Helpers.GenerateKey();
            admin1.Password = Helpers.Encrypt("P@ssw0rd", admin1.PrivateKey);
            admin1.IsLocked = false;
            admin1.IsActive = true;
            admin1.IsDeleted = false;
            admin1.CreateDate = DateTime.Now;
            admin1.Email = "admin@mail.com";
            admin1.FullName = "Administrator";
            IDFManager.Context.Users.Add(admin1);

            //App1
            App app1 = new App();
            app1.Id = StaticClass.app1Id;
            app1.Title = "App 1";
            app1.IsActive = true;
            app1.OwnerId = admin1.Id;
            IDFManager.Context.Apps.Add(app1);
            IDFManager.Context.SaveChanges();

            //AppPermissionType
            AppPermissionType appPermissionType1 = new();
            appPermissionType1.Id = StaticClass.app1PermissionType1Id;
            appPermissionType1.AppId = app1.Id;
            appPermissionType1.Title = "App1 PermissionType1";
            IDFManager.Context.AppPermissionTypes.Add(appPermissionType1);
            IDFManager.Context.SaveChanges();

            //Create AppAccess 1
            AppAccess appAccess1 = new();
            appAccess1.Id = Guid.NewGuid().ToString();
            appAccess1.AppId = app1.Id;
            appAccess1.UserId = admin1.Id;
            appAccess1.AccessKey = StaticClass.appAccess1Key;
            appAccess1.AccessSecret = StaticClass.appAccess1Secret;
            appAccess1.ExpireDate = DateTime.Now.AddYears(1);
            IDFManager.Context.AppAccess.Add(appAccess1);
            IDFManager.Context.SaveChanges();
            //Create AppAccess 2
            AppAccess appAccess2 = new();
            appAccess2.Id = Guid.NewGuid().ToString();
            appAccess2.AppId = app1.Id;
            appAccess2.UserId = admin1.Id;
            appAccess2.AccessKey = StaticClass.appAccess1Key;
            appAccess2.AccessSecret = StaticClass.appAccess1Secret;
            appAccess2.ExpireDate = DateTime.Now.AddYears(1);
            IDFManager.Context.AppAccess.Add(appAccess2);
            IDFManager.Context.SaveChanges();

            //Assign App Permission: AppAccessPermission
            AppAccessPermission appAccessPermission1 = new();
            appAccessPermission1.Id = Guid.NewGuid().ToString();
            appAccessPermission1.AppPermissionTypeId = appPermissionType1.Id;
            appAccessPermission1.AppAccessId = appAccess1.Id;
            IDFManager.Context.AppAccessPermissions.Add(appAccessPermission1);
            IDFManager.Context.SaveChanges();

            User user1 = new User();
            user1.Id = Guid.NewGuid().ToString();
            user1.UserName = "Username1";
            user1.PrivateKey = Helpers.GenerateKey();
            user1.Password = Helpers.Encrypt("P@ssw0rd", user1.PrivateKey);
            user1.IsLocked = false;
            user1.IsActive = true;
            user1.IsDeleted = false;
            user1.CreateDate = DateTime.Now;
            user1.Email = "username1@mail.com";
            user1.FullName = "Full Name 1";
            IDFManager.Context.Users.Add(user1);
            IDFManager.Context.SaveChanges();

            //Add Roles
            string newRoleId1 = Guid.NewGuid().ToString();
            IDFManager.Context.Roles.Add(new Role { Id = newRoleId1, Title = "Role 1" });
            string newRoleId2 = Guid.NewGuid().ToString();
            IDFManager.Context.Roles.Add(new Role { Id = newRoleId2, Title = "Role 2" });
            IDFManager.Context.SaveChanges();

            //Add Entities
            string newEntityId1 = Guid.NewGuid().ToString();
            IDFManager.Context.Entities.Add(new Entity { Id = newEntityId1, Title = "Entity 1" });
            string newEntityId2 = Guid.NewGuid().ToString();
            IDFManager.Context.Entities.Add(new Entity { Id = newEntityId2, Title = "Entity 2" });
            IDFManager.Context.SaveChanges();

            //Add PermissionTypes
            string newPermissionTypeId1 = Guid.NewGuid().ToString();
            IDFManager.Context.PermissionTypes.Add(new PermissionType { Id = newPermissionTypeId1, Title = "PermissionType 1" });
            string newPermissionTypeId2 = Guid.NewGuid().ToString();
            IDFManager.Context.PermissionTypes.Add(new PermissionType { Id = newPermissionTypeId2, Title = "PermissionType 2" });
            IDFManager.Context.SaveChanges();

            //Add Permissions
            string newPermissionId1 = Guid.NewGuid().ToString();
            IDFManager.Context.Permissions.Add(new Permission {Id = newPermissionId1, EntityId = newEntityId1, PermissionTypeId = newPermissionTypeId1, RoleId = newRoleId1 });
            string newPermissionId2 = Guid.NewGuid().ToString();
            IDFManager.Context.Permissions.Add(new Permission { Id = newPermissionId2, EntityId = newEntityId2, PermissionTypeId = newPermissionTypeId1, RoleId = newRoleId1 });
            IDFManager.Context.SaveChanges();

        }
    }
}

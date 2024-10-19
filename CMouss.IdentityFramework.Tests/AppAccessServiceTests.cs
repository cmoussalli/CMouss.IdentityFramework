using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using CMouss.IdentityFramework;
using Microsoft.EntityFrameworkCore;

namespace CMouss.IdentityFramework.Tests
{
    [TestClass]
    public class AppAccessService_Tests
    {
        IDFDBContext db = new IDFDBContext();



        [TestMethod]
        public void GetAppPermissionTypes_Ok()
        {
            List<AppPermissionType> appPermissionTypes = IDFManager.appAccessService.GetAppPermissionTypes("123456-App1AppAccess1Key", "123456-App1AppAccess1Secret");

            Assert.AreEqual(appPermissionTypes.Count, 1);
        }



        [TestMethod]
        public void GrantPermission_Ok()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "User X AppAccess.GrantPermission_Ok";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "AppAccess.GrantPermission_Ok@mail.com";
            user.FullName = "User X AppAccess.GrantPermission_Ok";
            db.Users.Add(user);
            db.SaveChanges();

            db = new IDFDBContext();
            AppPermissionType appPermissionType = new();
            string appPermissionTypeId = Guid.NewGuid().ToString();
            appPermissionType.Id = appPermissionTypeId;
            appPermissionType.Title = "PermissionType X AppAccess.GrantPermission_Ok";
            appPermissionType.AppId = StaticClass.app1Id;
            db.AppPermissionTypes.Add(appPermissionType);
            db.SaveChanges();

            db = new IDFDBContext();
            AppAccess appAccess = new ();
            string appAccessId = Guid.NewGuid().ToString();
            appAccess.Id = appAccessId;
            appAccess.AppId = StaticClass.app1Id;
            appAccess.ExpireDate = DateTime.Now.AddYears(1);
            appAccess.AccessKey = Guid.NewGuid().ToString();
            appAccess.AccessSecret = Guid.NewGuid().ToString();
            appAccess.UserId = newUserId;
            db.AppAccess.Add(appAccess);
            db.SaveChanges();

            IDFManager.appAccessService.GrantPermission(appAccessId,appPermissionTypeId);

            db = new IDFDBContext();
            List<AppAccessPermission> res = db.AppAccessPermissions.Where(o =>
                o.AppAccessId == appAccessId
                && o.AppPermissionTypeId == appPermissionTypeId
                ).ToList();
                
            Assert.AreEqual(res.Count, 1);
        }

        [TestMethod]
        public void  RevokePermission_Ok()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "User X AppAccess.RevokePermission_Ok";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "AppAccess.RevokePermission_Ok@mail.com";
            user.FullName = "User X AppAccess.RevokePermission_Ok";
            db.Users.Add(user);
            db.SaveChanges();

            db = new IDFDBContext();
            AppPermissionType appPermissionType = new();
            string appPermissionTypeId = Guid.NewGuid().ToString();
            appPermissionType.Id = appPermissionTypeId;
            appPermissionType.Title = "PermissionType X AppAccess.RevokePermission_Ok";
            appPermissionType.AppId = StaticClass.app1Id;
            db.AppPermissionTypes.Add(appPermissionType);
            db.SaveChanges();

            db = new IDFDBContext();
            AppAccess appAccess = new();
            string appAccessId = Guid.NewGuid().ToString();
            appAccess.Id = appAccessId;
            appAccess.AppId = StaticClass.app1Id;
            appAccess.ExpireDate = DateTime.Now.AddYears(1);
            appAccess.AccessKey = Guid.NewGuid().ToString();
            appAccess.AccessSecret = Guid.NewGuid().ToString();
            appAccess.UserId = newUserId;
            db.AppAccess.Add(appAccess);
            db.SaveChanges();

            db = new IDFDBContext();
            AppAccessPermission appAccessPermission = new();
            appAccessPermission.Id = Guid.NewGuid().ToString();
            appAccessPermission.AppPermissionTypeId = appPermissionTypeId;
            appAccessPermission.AppAccessId = appAccessId;
            db.AppAccessPermissions.Add(appAccessPermission);
            db.SaveChanges();

            db = new IDFDBContext();
            db = new IDFDBContext();
            List<AppAccessPermission> before = db.AppAccessPermissions.Where(o =>
                o.AppAccessId == appAccessId
                && o.AppPermissionTypeId == appPermissionTypeId
                ).ToList();

            IDFManager.appAccessService.RevokePermission(appAccessId, appPermissionTypeId);

            db = new IDFDBContext();
            List<AppAccessPermission> after = db.AppAccessPermissions.Where(o =>
                o.AppAccessId == appAccessId
                && o.AppPermissionTypeId == appPermissionTypeId
                ).ToList();

            Assert.AreEqual(before.Count,after.Count + 1);
        }

        [TestMethod]
        public void ValidateAppAccessPermission_Ok()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "User X AppAccess.ValidateAppAccessPermission_Ok";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "AppAccess.ValidateAppAccessPermission_Ok@mail.com";
            user.FullName = "User X AppAccess.ValidateAppAccessPermission_Ok";
            db.Users.Add(user);
            db.SaveChanges();

            //Create App
            db = new IDFDBContext();
            string appId = Helpers.GenerateId();
            App app = new();
            app.Id = appId;
            app.Title = "App X AppAcces.ValidateAppAccessPermission_Ok";
            app.OwnerId = newUserId;
            app.IsActive = true;
            app.IsDeleted = false;
            db.Apps.Add(app);
            db.SaveChanges();

            //Create AppPermissionType
            db = new IDFDBContext();
            AppPermissionType appPermissionType = new();
            string appPermissionTypeId = Guid.NewGuid().ToString();
            appPermissionType.Id = appPermissionTypeId;
            appPermissionType.Title = "PermissionType X AppAccess.ValidateAppAccessPermission_Ok";
            appPermissionType.AppId = appId;
            db.AppPermissionTypes.Add(appPermissionType);
            db.SaveChanges();

            //Create AppAccess
            db = new IDFDBContext();
            AppAccess appAccess = new();
            string appAccessId = Guid.NewGuid().ToString();
            appAccess.Id = appAccessId;
            appAccess.AppId = appId;
            appAccess.ExpireDate = DateTime.Now.AddYears(1);
            appAccess.AccessKey = Guid.NewGuid().ToString();
            appAccess.AccessSecret = Guid.NewGuid().ToString();
            appAccess.UserId = newUserId;
            db.AppAccess.Add(appAccess);
            db.SaveChanges();

            //Create AppAccessPermission
            db = new IDFDBContext();
            AppAccessPermission appAccessPermission = new();
            appAccessPermission.Id = Guid.NewGuid().ToString();
            appAccessPermission.AppPermissionTypeId = appPermissionTypeId;
            appAccessPermission.AppAccessId = appAccessId;
            db.AppAccessPermissions.Add(appAccessPermission);
            db.SaveChanges();


            bool result =  IDFManager.appAccessService.ValidateAppAccessPermission(appAccessId, appPermissionTypeId);

            db = new IDFDBContext();
            List<AppAccessPermission> after = db.AppAccessPermissions.Where(o =>
                o.AppAccessId == appAccessId
                && o.AppPermissionTypeId == appPermissionTypeId
                ).ToList();


            Assert.AreEqual(after.Count, 1);
            Assert.IsTrue(result);
        }
    }
}

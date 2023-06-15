using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using CMouss.IdentityFramework;
using Microsoft.EntityFrameworkCore;

namespace CMouss.IdentityFramework.Tests
{
    [TestClass]
    public class UserService_Tests
    {
        IDFDBContext db = new IDFDBContext();

        [TestMethod]
        public void Find_Exists()
        {
            string newId = IDFManager.UserServices.Create("UserTestExists_True", "P@ssw0rd", "User Test Exists_True", "UserTestExists_True@mail.com");

            db = new IDFDBContext();
            User o = db.Users.Find(newId);
            Assert.IsNotNull(Helpers.Decrypt(o.Password, o.PrivateKey), "P@ssw0rd");
        }
        [TestMethod]
        public void Find_NotExists()
        {
            string newId =  IDFManager.UserServices.Create("UserTestExists_False", "P@ssw0rd", "User Test Exists_False", "UserTestExists_False@mail.com");

            db = new IDFDBContext();
            try
            {
                User o = db.Users.Find(Guid.NewGuid().ToString());
                Assert.IsNull(o);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Not found");
            }

        }



        [TestMethod]
        public void Create()
        {
            string newId = IDFManager.UserServices.Create("UserTestCreate", "P@ssw0rd", "UserTest Create", "UserTestCreate@mail.com",false,true);

            db = new IDFDBContext();
            User o = db.Users.Find(newId);
            Assert.AreEqual(Helpers.Decrypt(o.Password, o.PrivateKey), "P@ssw0rd");

        }

        //[TestMethod]
        //public void Search()
        //{
        //    List<User> users = IDFManager.UserServices.Search(new SearchModels.UsersSearchModel {UserName = "Admin" });

        //    Assert.IsTrue( users.Count == 2);

        //}



        //[TestMethod]
        //public void Update()
        //{
        //    string newId = Guid.NewGuid().ToString();
        //    db.Users.Add(new User { Id= newId, Title ="User X UpdateTest" ,IsDeleted = false });
        //    db.SaveChanges();
        //    IDFManager.UserServices.Update(newId, "User X UpdateTestOK");

        //    db = new IDFDBContext();
        //    User o = db.Users.Find(newId);
        //    Assert.AreEqual( "User X UpdateTestOK", o.Title);

        //}

        //[TestMethod]
        //public void Delete()
        //{
        //    string newId = Guid.NewGuid().ToString();
        //    db.Users.Add(new User { Id = newId, Title = "User X DeleteTest", IsDeleted = false });
        //    db.SaveChanges();
        //    IDFManager.UserServices.Delete(newId);

        //    db = new IDFDBContext();
        //    User o = db.Users.Find(newId);
        //    Assert.AreEqual(o.IsDeleted, true);
        //}

        //[TestMethod]
        //public void Find()
        //{
        //    string newId = Guid.NewGuid().ToString();
        //    db.Users.Add(new User { Id = newId, Title = "User X FindTest", IsDeleted = false });
        //    db.SaveChanges();
        //    IDFManager.UserServices.Find(newId);

        //    db = new IDFDBContext();
        //    User o = db.Users.Find(newId);
        //    Assert.IsNotNull(o);
        //}

        //[TestMethod]
        //public void GetAll()
        //{
        //    string newId = Guid.NewGuid().ToString();
        //    db.Users.Add(new User { Id = newId, Title = "User X GetAllTest", IsDeleted = false });
        //    List<User> lst = IDFManager.UserServices.GetAll();
        //    Assert.IsTrue(lst.Count > 0);
        //}

        //[TestMethod]
        //public void ValidateLogin_Bool_True()
        //{
        //    bool isLogin = IDFManager.UserServices.Login("Username1", "P@ssw0rd");
        //    Assert.IsTrue(isLogin);
        //}
        //[TestMethod]
        //public void ValidateLogin_Bool_WrongPassword()
        //{
        //    bool isLogin = IDFManager.UserServices.Login("Username1", "WrongPassword");
        //    Assert.IsFalse(isLogin);
        //}
        //[TestMethod]
        //public void ValidateLogin_Bool_UsernameNotFound()
        //{
        //    bool isLogin = IDFManager.UserServices.Login("WrongUserName", "P@ssw0rd");
        //    Assert.IsFalse(isLogin);
        //}


        [TestMethod]
        public void ChangePassword_Ok()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "UserXChangePassword_Ok";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "ChangePassword_Ok@mail.com";
            user.FullName = "User X ChangePassword_Ok";
            db.Users.Add(user);
            db.SaveChanges();

            IDFManager.UserServices.ChangePassword(newUserId, "NewP@ssw0rd", false);
        }

        [TestMethod]
        public void ChangeMyPassword_Ok()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "UserXChangeMyPassword_Ok";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "ChangeMyPassword_Ok@mail.com";
            user.FullName = "User X ChangeMyPassword_Ok";
            db.Users.Add(user);
            db.SaveChanges();

            IDFManager.UserServices.ChangeMyPassword("UserXChangeMyPassword_Ok", "P@ssw0rd","NewP@ssw0rd",false);
        }

        [TestMethod]
        public void UserLogin_Ok()
        {
            UserToken t = IDFManager.AuthService.UserLogin("Username1", "P@ssw0rd");
            Assert.IsNotNull(t);
        }
        [TestMethod]
        public void UserLogin_UserNameNotFound()
        {
            try
            {
                UserToken t = IDFManager.AuthService.UserLogin("WrongUsername", "P@ssw0rd");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                if (ex.Message == Messages.NotFound)
                {
                    Assert.IsTrue(true);
                    return;
                }
            }
            Assert.IsTrue(false);
        }
        [TestMethod]
        public void UserLogin_WrongPassword()
        {
            try
            {
                UserToken t = IDFManager.AuthService.UserLogin("Username1", "WrongP@ssw0rd");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                if (ex.Message == "Incorrect password")
                {
                    Assert.IsTrue(true);
                    return;
                }
            }
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void GrantRole_Ok()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "User X GrantRole_Ok";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "UserXGrantRole_Ok@mail.com";
            user.FullName = "User X GrantRole_Ok";
            db.Users.Add(user);
            db.SaveChanges();

            db = new IDFDBContext();
            string newRoleId = Guid.NewGuid().ToString();
            db.Roles.Add(new Role { Id = newRoleId, Title = "Role X User.GrantRole_Ok" });
            db.SaveChanges();

            IDFManager.UserServices.GrantRole(newUserId, newRoleId);

            db = new IDFDBContext();
            User u = db.Users.Include(o => o.Roles).Where(o => o.Id == newUserId).ToList()[0];
            Assert.AreEqual(u.Roles.Count, 1);
        }

        [TestMethod]
        public void RevokeRole_Ok()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "User X RevokeRole_Ok";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "UserXRevokeRole_Ok@mail.com";
            user.FullName = "User X RevokeRole_Ok";
            db.Users.Add(user);
            db.SaveChanges();

            db = new IDFDBContext();
            string newRoleId = Guid.NewGuid().ToString();
            db.Roles.Add(new Role { Id = newRoleId, Title = "Role X User.RevokeRole_Ok" });
            db.SaveChanges();

            IDFManager.UserServices.GrantRole(newUserId, newRoleId);

            IDFManager.UserServices.RevokeRole(newUserId, newRoleId);

            db = new IDFDBContext();
            User u = db.Users.Include(o => o.Roles).Where(o => o.Id == newUserId).ToList()[0];
            Assert.AreEqual(u.Roles.Count, 0);
        }



        [TestMethod]
        public void GetUserPermissions_Ok()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "User X GetUserPermissions_Ok";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "UserXGetUserPermissions_Ok@mail.com";
            user.FullName = "User X GetUserPermissions_Ok";
            db.Users.Add(user);
            db.SaveChanges();

            //Add Roles
            db = new IDFDBContext();
            string newRoleId1 = Guid.NewGuid().ToString();
            db.Roles.Add(new Role { Id = newRoleId1, Title = "Role X1 User.GetUserPermissions_Ok" });
            db.SaveChanges();

            db = new IDFDBContext();
            string newRoleId2 = Guid.NewGuid().ToString();
            db.Roles.Add(new Role { Id = newRoleId2, Title = "Role X2 User.GetUserPermissions_Ok" });
            db.SaveChanges();

            IDFManager.UserServices.GrantRole(newUserId, newRoleId1);
            IDFManager.UserServices.GrantRole(newUserId, newRoleId2);

            //Add Entities
            string newEntityId1 = Guid.NewGuid().ToString();
            db.Entities.Add(new Entity { Id = newEntityId1, Title = "Entity X1 User.GetUserPermissions_Ok" });
            string newEntityId2 = Guid.NewGuid().ToString();
            db.Entities.Add(new Entity { Id = newEntityId2, Title = "Entity X2 User.GetUserPermissions_Ok" });
            db.SaveChanges();

            //Add PermissionTypes
            string newPermissionTypeId1 = Guid.NewGuid().ToString();
            db.PermissionTypes.Add(new PermissionType { Id = newPermissionTypeId1, Title = "PermissionType X1 User.GetUserPermissions_Ok" });
            string newPermissionTypeId2 = Guid.NewGuid().ToString();
            db.PermissionTypes.Add(new PermissionType { Id = newPermissionTypeId2, Title = "PermissionType X2 User.GetUserPermissions_Ok" });
            db.SaveChanges();

            //Add Permissions
            string newPermissionId1 = Guid.NewGuid().ToString();
            db.Permissions.Add(new Permission { Id = newPermissionId1, EntityId = newEntityId1, PermissionTypeId = newPermissionTypeId1, RoleId = newRoleId1 });
            string newPermissionId2 = Guid.NewGuid().ToString();
            db.Permissions.Add(new Permission { Id = newPermissionId2, EntityId = newEntityId1, PermissionTypeId = newPermissionTypeId2, RoleId = newRoleId1 });

            string newPermissionId3 = Guid.NewGuid().ToString();
            db.Permissions.Add(new Permission { Id = newPermissionId3, EntityId = newEntityId2, PermissionTypeId = newPermissionTypeId1, RoleId = newRoleId1 });
            string newPermissionId4 = Guid.NewGuid().ToString();
            db.Permissions.Add(new Permission { Id = newPermissionId4, EntityId = newEntityId1, PermissionTypeId = newPermissionTypeId1, RoleId = newRoleId2 });

            db.SaveChanges();

            List<EntityAllowedActionsModel> allowedEntities = IDFManager.UserServices.GetUserPermissions(newUserId);
            Assert.AreEqual(allowedEntities.Count, 2);
            Assert.AreEqual(allowedEntities[0].AllowedActions.Count, 2);
        }

        [TestMethod]
        public void GetUserEntityPermissions_Ok()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "User X GetUserPermissions_Ok";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "UserXGetUserPermissions_Ok@mail.com";
            user.FullName = "User X GetUserPermissions_Ok";
            db.Users.Add(user);
            db.SaveChanges();

            //Add Roles
            db = new IDFDBContext();
            string newRoleId1 = Guid.NewGuid().ToString();
            db.Roles.Add(new Role { Id = newRoleId1, Title = "Role X1 User.GetUserPermissions_Ok" });
            db.SaveChanges();

            db = new IDFDBContext();
            string newRoleId2 = Guid.NewGuid().ToString();
            db.Roles.Add(new Role { Id = newRoleId2, Title = "Role X2 User.GetUserPermissions_Ok" });
            db.SaveChanges();

            IDFManager.UserServices.GrantRole(newUserId, newRoleId1);
            IDFManager.UserServices.GrantRole(newUserId, newRoleId2);

            //Add Entities
            string newEntityId1 = Guid.NewGuid().ToString();
            db.Entities.Add(new Entity { Id = newEntityId1, Title = "Entity X1 User.GetUserPermissions_Ok" });
            string newEntityId2 = Guid.NewGuid().ToString();
            db.Entities.Add(new Entity { Id = newEntityId2, Title = "Entity X2 User.GetUserPermissions_Ok" });
            db.SaveChanges();

            //Add PermissionTypes
            string newPermissionTypeId1 = Guid.NewGuid().ToString();
            db.PermissionTypes.Add(new PermissionType { Id = newPermissionTypeId1, Title = "PermissionType X1 User.GetUserPermissions_Ok" });
            string newPermissionTypeId2 = Guid.NewGuid().ToString();
            db.PermissionTypes.Add(new PermissionType { Id = newPermissionTypeId2, Title = "PermissionType X2 User.GetUserPermissions_Ok" });
            db.SaveChanges();

            //Add Permissions
            string newPermissionId1 = Guid.NewGuid().ToString();
            db.Permissions.Add(new Permission { Id = newPermissionId1, EntityId = newEntityId1, PermissionTypeId = newPermissionTypeId1, RoleId = newRoleId1 });
            string newPermissionId2 = Guid.NewGuid().ToString();
            db.Permissions.Add(new Permission { Id = newPermissionId2, EntityId = newEntityId2, PermissionTypeId = newPermissionTypeId1, RoleId = newRoleId1 });

            string newPermissionId3 = Guid.NewGuid().ToString();
            db.Permissions.Add(new Permission { Id = newPermissionId3, EntityId = newEntityId1, PermissionTypeId = newPermissionTypeId1, RoleId = newRoleId2 });

            db.SaveChanges();

            //db = new IDFDBContext();
            //User u = db.Users.Include(o => o.Roles).Where(o => o.Id == newUserId).ToList()[0];
            //List<Permission> permissions = db.Permissions.Where(o => u.Roles)
            List<EntityAllowedActionsModel> allowedActions = IDFManager.UserServices.GetUserPermissions(newUserId);
            Assert.AreEqual(allowedActions.Count, 2);
        }

        [TestMethod]
        public void ValidateUserPermission_Ok()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "User X ValidateUserPermission_Ok";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "ValidateUserPermission_Ok@mail.com";
            user.FullName = "User X ValidateUserPermission_Ok";
            db.Users.Add(user);
            db.SaveChanges();

            //Create Role
            db = new IDFDBContext();
            string newRoleId = Guid.NewGuid().ToString();
            Role newRole = new Role { Id = newRoleId, Title = "Role X User.ValidateUserPermission_Ok" };
            db.Roles.Add(newRole);
            db.SaveChanges();
            //Attached Role to User
            //user.Roles.Add(newRole);
            //db.SaveChanges();
            IDFManager.UserServices.GrantRole(newUserId, newRoleId);

            //Add Entity
            db = new IDFDBContext();
            string newEntityId1 = Guid.NewGuid().ToString();
            db.Entities.Add(new Entity { Id = newEntityId1, Title = "Entity X1 User.ValidateUserPermission_Ok" });
            db.SaveChanges();

            //Add PermissionTypes
            db = new IDFDBContext();
            string newPermissionTypeId1 = Guid.NewGuid().ToString();
            db.PermissionTypes.Add(new PermissionType { Id = newPermissionTypeId1, Title = "PermissionType X1 User.ValidateUserPermission_Ok" });
            db.SaveChanges();

            //Add the permission
            db = new IDFDBContext();
            string newPermissionId1 = Guid.NewGuid().ToString();
            db.Permissions.Add(new Permission { Id = newPermissionId1, EntityId = newEntityId1, PermissionTypeId = newPermissionTypeId1, RoleId = newRoleId });
            db.SaveChanges();

            //Exec evaluated Method      
            db = new IDFDBContext();
            bool eval = IDFManager.UserServices.ValidateUserPermission(newUserId, newEntityId1, newPermissionTypeId1);
            Assert.IsTrue(eval);
        }

        [TestMethod]
        public void ValidateUserRole_Ok()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "User X ValidateUserRole_Ok";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "ValidateUserRole_Ok@mail.com";
            user.FullName = "User X ValidateUserRole_Ok";
            db.Users.Add(user);
            db.SaveChanges();

            //Create Role1
            db = new IDFDBContext();
            string newRoleId1 = Guid.NewGuid().ToString();
            Role newRole1 = new Role { Id = newRoleId1, Title = "Role X User.ValidateUserRole_Ok" };
            db.Roles.Add(newRole1);
            db.SaveChanges();
            //Create Role2
            db = new IDFDBContext();
            string newRoleId2 = Guid.NewGuid().ToString();
            Role newRole2 = new Role { Id = newRoleId2, Title = "Role Y User.ValidateUserRole_Ok" };
            db.Roles.Add(newRole2);
            db.SaveChanges();
            //Attached Role to User
            //user.Roles.Add(newRole);
            //db.SaveChanges();
            IDFManager.UserServices.GrantRole(newUserId, newRoleId2);


            //Exec evaluated Method      
            db = new IDFDBContext();
            bool eval = IDFManager.UserServices.ValidateUserRole(newUserId, newRoleId2);
            Assert.IsTrue(eval);
        }

        [TestMethod]
        public void ValidateUserRoles_Ok()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "User X ValidateUserRoles_Ok";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "ValidateUserRoles_Ok@mail.com";
            user.FullName = "User X ValidateUserRoles_Ok";
            db.Users.Add(user);
            db.SaveChanges();

            //Create Role1
            db = new IDFDBContext();
            string newRoleId1 = Guid.NewGuid().ToString();
            Role newRole1 = new Role { Id = newRoleId1, Title = "Role X User.ValidateUserRoles_Ok" };
            db.Roles.Add(newRole1);
            db.SaveChanges();
            //Create Role2
            db = new IDFDBContext();
            string newRoleId2 = Guid.NewGuid().ToString();
            Role newRole2 = new Role { Id = newRoleId2, Title = "Role Y User.ValidateUserRoles_Ok" };
            db.Roles.Add(newRole2);
            db.SaveChanges();
            //Attached Role to User
            //user.Roles.Add(newRole);
            //db.SaveChanges();
            IDFManager.UserServices.GrantRole(newUserId, newRoleId2);


            //Exec evaluated Method      
            db = new IDFDBContext();
            List<string> rolesEval = new() { "123",newRoleId2};
            bool eval = IDFManager.UserServices.ValidateUserRole(newUserId, rolesEval);
            Assert.IsTrue(eval);
        }

    }

}

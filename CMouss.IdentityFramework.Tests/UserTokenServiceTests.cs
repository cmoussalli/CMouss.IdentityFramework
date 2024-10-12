using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using CMouss.IdentityFramework;
using Microsoft.EntityFrameworkCore;

namespace CMouss.IdentityFramework.Tests
{
    [TestClass]
    public class UserTokenService_Tests
    {
        IDFDBContext db = new IDFDBContext();


        #region Create

        [TestMethod]
        public void Create_Ok()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "User X Token.Create_Ok";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "UserXToken.Create_Ok@mail.com";
            user.FullName = "User X Token.Create_Ok";
            db.Users.Add(user);
            db.SaveChanges();

            IDFManager.UserTokenServices.Create(newUserId, new LifeTime(1, 0, 0),"");

            db = new IDFDBContext();
            UserToken t = db.UserTokens.Where(o =>
            o.UserId == newUserId
            ).ToList()[0];
            Assert.IsNotNull(t);
        }
        #endregion


        #region Validate

        [TestMethod]
        public void Validate_Ok()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "User X Token.Validate_Ok";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "UserXToken.Validate_Ok@mail.com";
            user.FullName = "User X Token.Validate_Ok";
            db.Users.Add(user);
            db.SaveChanges();

            string newToken = Helpers.GenerateKey();
            UserToken token = new UserToken();
            token.Token = newToken;
            token.UserId = newUserId;
            token.ExpireDate = DateTime.Now.AddHours(1);
            db.UserTokens.Add(token);
            db.SaveChanges();

            UserToken verify = IDFManager.UserTokenServices.Validate(newToken,TokenValidationMode.UseDefault, "");

            Assert.IsNotNull(verify);
        }

        [TestMethod]
        public void Validate_Expired()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "User X Token.Validate_Expire";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "UserXToken.Validate_Expire@mail.com";
            user.FullName = "User X Token.Validate_Expire";
            db.Users.Add(user);
            db.SaveChanges();

            string newToken = Helpers.GenerateKey();
            UserToken token = new UserToken();
            token.Token = newToken;
            token.UserId = newUserId;
            token.ExpireDate = DateTime.UtcNow.AddMinutes(-1);
            db.UserTokens.Add(token);
            db.SaveChanges();

            UserToken verify = IDFManager.UserTokenServices.Validate(newToken,TokenValidationMode.UseDefault,"");

            Assert.IsNull(verify);
        }

        #endregion

        #region Delete

        [TestMethod]
        public void Delete_Ok()
        {
            User user = new User();
            string newUserId = Guid.NewGuid().ToString();
            user.Id = newUserId;
            user.UserName = "User X Token.Delete_Ok";
            user.PrivateKey = Helpers.GenerateKey();
            user.Password = Helpers.Encrypt("P@ssw0rd", user.PrivateKey);
            user.IsLocked = false;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreateDate = DateTime.Now;
            user.Email = "UserXToken.Delete_Ok@mail.com";
            user.FullName = "User X Token.Delete_Ok";
            db.Users.Add(user);
            db.SaveChanges();

            string newToken = Helpers.GenerateKey();
            UserToken token = new UserToken();
            token.Token = newToken;
            token.UserId = newUserId;
            token.ExpireDate = DateTime.Now.AddHours(1);
            db.UserTokens.Add(token);
            db.SaveChanges();

            IDFManager.UserTokenServices.Delete(newToken);


        }

      

        #endregion


    }
}

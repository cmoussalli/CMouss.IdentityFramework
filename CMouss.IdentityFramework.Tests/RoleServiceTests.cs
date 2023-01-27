using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using CMouss.IdentityFramework;

namespace CMouss.IdentityFramework.Tests
{
    [TestClass]
    public class RoleService_Tests
    {
        IDFDBContext db = new IDFDBContext();


        [TestMethod]
        public void Create()
        {
            string newId = Guid.NewGuid().ToString();
            IDFManager.RoleServices.Create(newId, "Role X CreateTest");

            Role o = db.Roles.Find(newId);
            Assert.IsNotNull(o);
        }

        [TestMethod]
        public void Update()
        {
            string newId = Guid.NewGuid().ToString();
            db.Roles.Add(new Role { Id= newId, Title ="Role X UpdateTest" });
            db.SaveChanges();
            IDFManager.RoleServices.Update(newId, "Role X UpdateTestOK");

            db = new IDFDBContext();
            Role o = db.Roles.Find(newId);
            Assert.AreEqual( "Role X UpdateTestOK", o.Title);
            
        }

        //[TestMethod]
        //public void Delete()
        //{
        //    string newId = Guid.NewGuid().ToString();
        //    db.Roles.Add(new Role { Id = newId, Title = "Role X DeleteTest", IsDeleted = false });
        //    db.SaveChanges();
        //    IDFManager.RoleServices.Delete(newId);

        //    db = new IDFDBContext();
        //    Role o = db.Roles.Find(newId);
        //    Assert.AreEqual(o.IsDeleted, true);
        //}

        [TestMethod]
        public void Find()
        {
            string newId = Guid.NewGuid().ToString();
            db.Roles.Add(new Role { Id = newId, Title = "Role X FindTest" });
            db.SaveChanges();
            IDFManager.RoleServices.Find(newId);

            db = new IDFDBContext();
            Role o = db.Roles.Find(newId);
            Assert.IsNotNull(o);
        }

        [TestMethod]
        public void GetAll()
        {
            string newId = Guid.NewGuid().ToString();
            db.Roles.Add(new Role { Id = newId, Title = "Role X GetAllTest" });
            List<Role> lst = IDFManager.RoleServices.GetAll();
            Assert.IsTrue(lst.Count > 0);
        }



    }
}

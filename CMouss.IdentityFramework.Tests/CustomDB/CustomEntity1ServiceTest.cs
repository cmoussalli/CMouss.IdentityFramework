using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using CMouss.IdentityFramework;

namespace CMouss.IdentityFramework.Tests
{
    [TestClass]
    public class CustomEntity1Service_Tests
    {
        CustomDBModel db = new CustomDBModel();


        [TestMethod]
        public void Method1()
        {
            string newId = Guid.NewGuid().ToString();
            CustomEntity1 o = new CustomEntity1();
            o.Id = newId;
            o.Title = "CustomEntity1 1";
            db.CustomEntity1s.Add(o);
            db.SaveChanges();

            CustomEntity1 t = db.CustomEntity1s.Find(newId);
            Assert.IsNotNull(o);
        }

   



    }
}

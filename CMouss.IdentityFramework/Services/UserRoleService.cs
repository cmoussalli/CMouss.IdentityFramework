//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CMouss.IdentityFramework
//{
//    internal class UserRoleService
//    {


//        internal List<RoleUser> GetAll()
//        {
//            return IDFManager.Context.RoleUsers.ToList();
//        }


//        internal void Create(string id, string entityId, string RoleUserTypeId)
//        {
//            RoleUser o = new RoleUser();
//            o.Id = id;
//            o.EntityId = entityId;
//            o.RoleUserTypeId = RoleUserTypeId;
//            IDFManager.Context.RoleUsers.Add(o);
//            IDFManager.Context.SaveChanges();
//        }

//        internal void Delete(string id)
//        {
//            List<RoleUser> lst = IDFManager.Context.RoleUsers.Where(o => o.Id == id).ToList();
//            //TODO: Delete RoleUser
//            IDFManager.Context.SaveChanges();
//        }



//    }
//}

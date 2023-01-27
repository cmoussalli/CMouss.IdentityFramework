using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class PermissionService
    {

        public Permission? Find(string id)
        {
            List<Permission> lst = IDFManager.Context.Permissions.Where(o => o.Id == id).ToList();
            if (lst.Count == 0)
            {
                return null;
            }
            return lst[0];
        }

        public List<Permission> GetAll()
        {
            return IDFManager.Context.Permissions.ToList();
        }


        public void Create(string id,string roleId, string entityId, string PermissionTypeId)
        {
            Permission o = new Permission();
            o.Id = id;
            o.RoleId = roleId;
            o.EntityId = entityId;
            o.PermissionTypeId = PermissionTypeId;
            IDFManager.Context.Permissions.Add(o);
            IDFManager.Context.SaveChanges();
        }

        public void Delete(string id)
        {
            List<Permission> lst = IDFManager.Context.Permissions.Where(o => o.Id == id).ToList();
            //TODO: Delete Permission
            IDFManager.Context.SaveChanges();
        }



    }
}

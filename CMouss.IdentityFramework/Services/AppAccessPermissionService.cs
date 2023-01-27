using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class AppAccessPermissionService
    {

        public AppAccessPermission? Find(string id)
        {
            List<AppAccessPermission> lst = IDFManager.Context.AppAccessPermissions.Where(o => o.Id == id).ToList();
            if (lst.Count == 0)
            {
                return null;
            }
            return lst[0];
        }

        public List<AppAccessPermission> GetAll()
        {
            return IDFManager.Context.AppAccessPermissions.ToList();
        }
        public List<AppAccessPermission> GetAll(string appAccessId)
        {
            return IDFManager.Context.AppAccessPermissions.Where(o => 
            o.AppAccessId == appAccessId
            ).ToList();
        }


        public void Create(string appAccessId, string appPermissionTypeId)
        {
            AppAccessPermission o = new AppAccessPermission();
            o.AppAccessId = appAccessId;
            o.AppPermissionTypeId = appPermissionTypeId;    
           
            IDFManager.Context.AppAccessPermissions.Add(o);
            IDFManager.Context.SaveChanges();
        }

        public void Delete(string id)
        {
            List<AppAccessPermission> lst = IDFManager.Context.AppAccessPermissions.Where(o => o.Id == id).ToList();
            //TODO: Delete AppAccessPermission
            IDFManager.Context.SaveChanges();
        }



    }
}

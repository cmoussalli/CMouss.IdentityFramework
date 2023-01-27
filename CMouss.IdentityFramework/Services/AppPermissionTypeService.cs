using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class AppAppPermissionTypeService
    {

        public AppPermissionType? Find(string id)
        {
            List<AppPermissionType> lst = IDFManager.Context.AppPermissionTypes.Where(o => o.Id == id).ToList();
            if (lst.Count == 0)
            {
                return null;
            }
            return lst[0];
        }

        public List<AppPermissionType> GetAll()
        {
            return IDFManager.Context.AppPermissionTypes.ToList();
        }
        public List<AppPermissionType> GetAll(string appId)
        {
            return IDFManager.Context.AppPermissionTypes.Where(o => 
            o.AppId == appId
            ).ToList();
        }


        public void Create(string id, string title,string appId)
        {
            AppPermissionType o = new AppPermissionType();
            o.Id = id;
            o.Title = title;
            o.AppId = appId;
            IDFManager.Context.AppPermissionTypes.Add(o);
            IDFManager.Context.SaveChanges();
        }

        public void Update(string id, string title,string appId)
        {
            List<AppPermissionType> lst = IDFManager.Context.AppPermissionTypes.Where(o => o.Id == id).ToList();
            if (lst.Count > 0)
            {
                lst[0].Title = title;
                lst[0].AppId = appId;
            }
            IDFManager.Context.SaveChanges();
        }

        public void Delete(string id)
        {
            List<AppPermissionType> lst = IDFManager.Context.AppPermissionTypes.Where(o => o.Id == id).ToList();
            //TODO: Delete AppPermissionType
            IDFManager.Context.SaveChanges();
        }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class PermissionTypeService
    {

        public PermissionType? Find(string id)
        {
            List<PermissionType> lst = IDFManager.Context.PermissionTypes.Where(o => o.Id == id).ToList();
            if (lst.Count == 0)
            {
                return null;
            }
            return lst[0];
        }

        public List<PermissionType> GetAll()
        {
            return IDFManager.Context.PermissionTypes.ToList();
        }


        public void Create(string id, string title)
        {
            PermissionType o = new PermissionType();
            o.Id = id;
            o.Title = title;
            IDFManager.Context.PermissionTypes.Add(o);
            IDFManager.Context.SaveChanges();
        }

        public void Update(string id, string title)
        {
            List<PermissionType> lst = IDFManager.Context.PermissionTypes.Where(o => o.Id == id).ToList();
            if (lst.Count > 0)
            {
                lst[0].Title = title;
            }
            IDFManager.Context.SaveChanges();
        }

        public void Delete(string id)
        {
            List<PermissionType> lst = IDFManager.Context.PermissionTypes.Where(o => o.Id == id).ToList();
            //TODO: Delete PermissionType
            IDFManager.Context.SaveChanges();
        }



    }
}

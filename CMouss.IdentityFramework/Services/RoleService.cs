using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CMouss.IdentityFramework
{
    public class RoleService
    {

        public Role? Find(string id)
        {
            List<Role> lst = IDFManager.Context.Roles.Where(o => o.Id == id).ToList();
            if(lst.Count == 0)
            {
                return null;
            }
            return lst[0];
        }

        public List<Role> GetAll()
        {
            Storage.Roles = IDFManager.Context.Roles.Include(o => o.Permissions).ToList();
            return Storage.Roles;
        }


        public void Create(string id, string title)
        {
            Role o = new Role();
            o.Id = id;
            o.Title = title;
            IDFManager.Context.Roles.Add(o);
            IDFManager.Context.SaveChanges();
        }

        public void Update(string id, string title)
        {
            List<Role> lst = IDFManager.Context.Roles.Where(o => o.Id == id).ToList();
            if(lst.Count>0)
            {
                lst[0].Title = title;
            }
            IDFManager.Context.SaveChanges();
        }

        public void Delete(string id)
        {
            //TODO: Delete Role
            List<Role> lst = IDFManager.Context.Roles.Where(o => o.Id == id).ToList();
            
            IDFManager.Context.SaveChanges();
        }



    }
}

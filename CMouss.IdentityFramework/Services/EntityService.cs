using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class EntityService
    {

        public Entity? Find(string id)
        {
            List<Entity> lst = IDFManager.Context.Entities.Where(o => o.Id == id).ToList();
            if (lst.Count == 0)
            {
                return null;
            }
            return lst[0];
        }

        public List<Entity> GetAll()
        {
            return IDFManager.Context.Entities.ToList();
        }


        public void Create(string id, string title)
        {
            Entity o = new Entity();
            o.Id = id;
            o.Title = title;
            IDFManager.Context.Entities.Add(o);
            IDFManager.Context.SaveChanges();
        }

        public void Update(string id, string title)
        {
            List<Entity> lst = IDFManager.Context.Entities.Where(o => o.Id == id).ToList();
            if (lst.Count > 0)
            {
                lst[0].Title = title;
            }
            IDFManager.Context.SaveChanges();
        }

        public void Delete(string id)
        {
            List<Entity> lst = IDFManager.Context.Entities.Where(o => o.Id == id).ToList();
            //TODO: Delete Entity
            IDFManager.Context.SaveChanges();
        }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework
{
    public class AttributeTypeService
    {

        public AttributeType? Find(string id)
        {
            List<AttributeType> lst = IDFManager.Context.AttributeTypes.Where(o => o.Id == id).ToList();
            if (lst.Count == 0)
            {
                return null;
            }
            return lst[0];
        }

        public List<AttributeType> GetAll()
        {
            return IDFManager.Context.AttributeTypes.ToList();
        }


        public void Create(string id, string title,bool isList)
        {
            AttributeType o = new AttributeType();
            o.Id = id;
            o.Title = title;
            o.IsList = isList;
            IDFManager.Context.AttributeTypes.Add(o);
            IDFManager.Context.SaveChanges();
        }

        public void Update(string id, string title,bool isList)
        {
            List<AttributeType> lst = IDFManager.Context.AttributeTypes.Where(o => o.Id == id).ToList();
            if (lst.Count > 0)
            {
                lst[0].Title = title;
                lst[0].IsList = isList;
            }
            IDFManager.Context.SaveChanges();
        }

        public void Delete(string id)
        {
            List<AttributeType> lst = IDFManager.Context.AttributeTypes.Where(o => o.Id == id).ToList();
            //TODO: Delete AttributeType
            IDFManager.Context.SaveChanges();
        }



    }
}

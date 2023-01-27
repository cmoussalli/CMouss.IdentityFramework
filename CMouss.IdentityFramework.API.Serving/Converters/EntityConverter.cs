using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIModels = CMouss.IdentityFramework.API.Models;
using DBModels = CMouss.IdentityFramework;

namespace CMouss.IdentityFramework.API.Serving.Converters
{
    public static class EntityConverter
    {
        public static APIModels.Entity ToAPIEntity(DBModels.Entity dbEntity)
        {
            APIModels.Entity apiEntity = new();
            apiEntity.Id = dbEntity.Id;
            apiEntity.Title = dbEntity.Title;
            return apiEntity;
        }

        public static List<APIModels.Entity> ToAPIEntitysList(List<DBModels.Entity> dbEntitys)
        {
            List<APIModels.Entity> EntityiEntitys = new();
            foreach(DBModels.Entity d in dbEntitys)
            {
                APIModels.Entity a = ToAPIEntity(d);
                EntityiEntitys.Add(a);
            }
            return EntityiEntitys;
        }
    }
}

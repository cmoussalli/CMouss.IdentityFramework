using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIModels = CMouss.IdentityFramework.API.Models;
using DBModels = CMouss.IdentityFramework;

namespace CMouss.IdentityFramework.API.Serving.Converters
{
    public static class PagingConverter
    {
        public static APIModels.Paging ToAPIPaging(DBModels.Paging paging)
        {
            APIModels.Paging apiPaging = new();
            apiPaging.PageNumber = paging.PageNumber;
            apiPaging.PageSize = paging.PageSize;


            return apiPaging;
        }

        public static DBModels.Paging ToDBPaging(APIModels.Paging paging)
        {
            DBModels.Paging dbPaging = new();
            dbPaging.PageNumber = paging.PageNumber;
            dbPaging.PageSize = paging.PageSize;


            return dbPaging;
        }
    }
}

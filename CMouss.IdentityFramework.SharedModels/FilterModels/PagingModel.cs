using System;
using System.Collections.Generic;
using System.Text;

namespace CMouss.IdentityFramework.SharedModels
{
    public class PagingSharedModel
    {

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;


        public int Skip
        {
            get
            {
                return (PageNumber - 1) * PageSize;
            }
        }



    }
}

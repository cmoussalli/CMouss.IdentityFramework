using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.API.Models
{
    public class IDFAuthResponseModels
    {

        public class IDFAuth : GenericResponseModel
        {
            public AuthResult AuthResult { get; set; }
        }


    }
}

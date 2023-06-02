using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMouss.IdentityFramework.API.Models;

namespace CMouss.IdentityFramework.API.Serving
{
    public class IDFBaseActionFilterAttribute : ActionFilterAttribute
    {

        public static void ReturnSecurityFail(ActionExecutingContext context, string content)
        {
            context.Result = new ContentResult
            {
                Content = content
                   ,
                StatusCode = 403
                   ,
                ContentType = "text/plain"
            };
            //context.Result = new StatusCodeResult(403); // Return a Forbidden HTTP status code

            return;
        }


    }
}

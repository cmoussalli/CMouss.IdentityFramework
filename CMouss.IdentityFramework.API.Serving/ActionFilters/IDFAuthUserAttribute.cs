using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMouss.IdentityFramework.API.Models;
using CMouss.IdentityFramework.Services;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CMouss.IdentityFramework.API.Serving
{



    public class IDFAuthUserAttribute : IDFBaseActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Request.Headers.TryGetValue("userToken", out var userToken);
            //context.HttpContext.Request.Headers.TryGetValue("appKey", out var appKey);
            //context.HttpContext.Request.Headers.TryGetValue("appSecret", out var appSecret);

            if (string.IsNullOrEmpty(userToken))
            {
                Helpers.ReturnSecurityFail(context, SecurityValidationResult.IncorrectParameters.ToString());
            }
            string ip = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            AuthResult authResult = IDFManager.AuthService.AuthUserToken(userToken.ToString(),ip);

            if (authResult.SecurityValidationResult == SecurityValidationResult.Ok)
            {
                context.ActionArguments["requesterAuthInfo"] = JsonConvert.SerializeObject(Converters.AuthResultConverter.ToAPIAuthResult(authResult));
            }
            else
            { Helpers.ReturnSecurityFail(context, authResult.SecurityValidationResult.ToString()); }

            base.OnActionExecuting(context);
        }




    }
}

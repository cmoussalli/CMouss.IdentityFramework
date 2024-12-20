﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using CMouss.IdentityFramework;
using CMouss.IdentityFramework.API.Models;

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
                ReturnSecurityFail(context, SecurityValidationResult.IncorrectParameters.ToString());
            }
            string ip = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            AuthResult authResult = IDFManager.authService.AuthUserToken(userToken.ToString(), TokenValidationMode.UseDefault, ip);

            if (authResult.SecurityValidationResult != SecurityValidationResult.Ok)
            {
                ReturnSecurityFail(context, authResult.SecurityValidationResult.ToString());
            }

            base.OnActionExecuting(context);
        }




    }
}

﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMouss.IdentityFramework.API.Models;
using Newtonsoft.Json;

namespace CMouss.IdentityFramework.API.Serving
{

    public class IDFAuthUserWithPermissionAttribute : IDFBaseActionFilterAttribute
    {

        private readonly string _entityId;
        private readonly string _permissionTypeId;

        /// <summary>
        /// Authenticate using User info only (UserToken). App authentication is not supported.
        /// Authorization is App based, multiple App is not supported with this function.
        /// Add "requesterAuthInfo" argument to controller action in order to get the AuthResult data.
        /// </summary>
        /// <param name="roleId"></param>
        public IDFAuthUserWithPermissionAttribute(string entityId, string permissionTypeId) =>
            (_entityId, _permissionTypeId) =
            (entityId, permissionTypeId);



        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Request.Headers.TryGetValue("userToken", out var userToken);

            if (string.IsNullOrEmpty(userToken))
            {
                ReturnSecurityFail(context, SecurityValidationResult.IncorrectParameters.ToString());
            }

            //string ip = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            AuthResult authResult = IDFManager.authService.AuthUserTokenWithPermission(userToken.ToString(), new EntityPermission(_entityId, _permissionTypeId));


            if (authResult.SecurityValidationResult != SecurityValidationResult.Ok)
            {
                ReturnSecurityFail(context, authResult.SecurityValidationResult.ToString());
            }

            base.OnActionExecuting(context);
        }




    }
}

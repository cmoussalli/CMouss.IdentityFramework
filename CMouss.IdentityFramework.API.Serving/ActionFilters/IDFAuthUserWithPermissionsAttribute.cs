using Microsoft.AspNetCore.Mvc.Filters;
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

    public class IDFAuthUserWithPermissionsAttribute : IDFBaseActionFilterAttribute
    {

        private readonly string _entityPermissions;

        public IDFAuthUserWithPermissionsAttribute(string entityPermissions) =>
            (_entityPermissions) =
            (entityPermissions);



        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Request.Headers.TryGetValue("userToken", out var userToken);

            if (string.IsNullOrEmpty(userToken))
            {
                ReturnSecurityFail(context, SecurityValidationResult.IncorrectParameters.ToString());
            }

            List<EntityPermission> permissions = new List<EntityPermission>();
            List<string> entityPermissionsStr = _entityPermissions.Split(",").ToList();
            foreach (string entityPermissionStr in entityPermissionsStr)
            {
                List<string> entityPermissionParts = entityPermissionStr.Split(":").ToList();
                if (entityPermissionParts.Count == 2)
                {
                    EntityPermission entityPermission = new() { EntityId = entityPermissionParts[0], PermissionTypeId = entityPermissionParts[1] };
                    permissions.Add(entityPermission);
                }
            }

            //string ip = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            AuthResult authResult = IDFManager.authService.AuthUserTokenWithPermissions(userToken.ToString(), permissions);


            if (authResult.SecurityValidationResult != SecurityValidationResult.Ok)
            {
                ReturnSecurityFail(context, authResult.SecurityValidationResult.ToString());
            }

            base.OnActionExecuting(context);
        }




    }
}

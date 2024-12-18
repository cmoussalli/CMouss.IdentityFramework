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



    public class IDFAuthUserWithRolesOrPermissionsAttribute : IDFBaseActionFilterAttribute
    {

        private readonly string _roleIds;
        private readonly string _entityPermissions;

        public IDFAuthUserWithRolesOrPermissionsAttribute(string roleIds, string entityPermissions) =>
            (_roleIds,_entityPermissions) =
            (roleIds, entityPermissions);



        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Request.Headers.TryGetValue("userToken", out var userToken);

            if (string.IsNullOrEmpty(userToken))
            {
                ReturnSecurityFail(context, SecurityValidationResult.IncorrectParameters.ToString());
            }

            List<EntityPermission> permissions = new List<EntityPermission>();
            List<string> roles = new();

            _roleIds.Split(",").ToList().ForEach(r => roles.Add(r.Trim()));

            foreach (string entityPermissionStr in _entityPermissions.Split(",").ToList())
            {
                List<string> entityPermissionParts = entityPermissionStr.Split(":").ToList();
                if (entityPermissionParts.Count == 2)
                {
                    EntityPermission entityPermission = new() { EntityId = entityPermissionParts[0], PermissionTypeId = entityPermissionParts[1] };
                    permissions.Add(entityPermission);
                }
            }



            AuthResult authResult = IDFManager.authService.AuthUserTokenWithPermissionsOrRoles(userToken.ToString(), permissions, roles );


            if (authResult.SecurityValidationResult != SecurityValidationResult.Ok)
            {
                ReturnSecurityFail(context, authResult.SecurityValidationResult.ToString());
            }

            base.OnActionExecuting(context);
        }




    }
}

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

    public class IDFAuthUserWithRolesAttribute : IDFBaseActionFilterAttribute
    {

        private readonly string _roleIds;
        /// <summary>
        /// Authenticate using User info only (UserToken). App authentication is not supported.
        /// Authorization is App based, multiple App is not supported with this function.
        /// Add "requesterAuthInfo" argument to controller action in order to get the AuthResult data.
        /// </summary>
        /// <param name="roleIds"></param>
        public IDFAuthUserWithRolesAttribute(string roleIds) =>
            (_roleIds) =
            (roleIds);

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Request.Headers.TryGetValue("userToken", out var userToken);

            if (string.IsNullOrEmpty(userToken))
            {
                ReturnSecurityFail(context, SecurityValidationResult.IncorrectParameters.ToString());
            }

            List<string> roles = new();
            _roleIds.Split(",").ToList().ForEach(r => roles.Add(r.Trim()));

            AuthResult authResult = IDFManager.authService.AuthUserTokenWithRoles(userToken.ToString(), roles);

            if (authResult.SecurityValidationResult != SecurityValidationResult.Ok)
            {
                ReturnSecurityFail(context, authResult.SecurityValidationResult.ToString());
            }

            base.OnActionExecuting(context);
        }

    }







}

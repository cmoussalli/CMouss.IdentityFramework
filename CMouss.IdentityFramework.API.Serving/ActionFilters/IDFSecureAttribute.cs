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

    /// <summary>
    /// Use Tenant mode only if you want to validate the authenticated app's both User permission and App Permission
    /// </summary>
    public enum AppPermissionMode
    {
        NotAllowed
            , SimpleMode
            , TenantMode
    }



    public static class ActionPermissionFactory
    {
        public static ActionPermission Create()
        {
            return new ActionPermission();
        }
    }


    /// <summary>
    /// Set The authorization rules
    /// </summary>
    public class ActionPermission
    {
        /// <summary>
        /// Specify the required granted entity access to user  
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// Specify the required granted entity permission type access to user  
        /// </summary>
        public string PermissionTypeId { get; set; }

        /// <summary>
        /// Specify the required app permission type, used only if AppPermissionMode: SimpleMode & TenantMode
        /// </summary>
        public string AppPermissionTypeId { get; set; }

        /// <summary>
        /// Specify how you want app authorization behave.
        /// NotAllowed: Ignore app permissions.
        /// SimpleMode: Validate app permission type only, IGNORE user entity and permission type rules.
        /// Tenant Mode: Validate BOTH app permission type and user entity and permission type rules.
        /// </summary>
        public AppPermissionMode AppPermissionMode { get; set; } = AppPermissionMode.NotAllowed;

        public ActionPermission()
        {
            EntityId = "";
            PermissionTypeId = "";
            AppPermissionTypeId = "";
        }

    }



    public class IDFAllowUserAndAppAttribute : ActionFilterAttribute
    {
        private readonly string _userEntityId;
        private readonly string _userPermissionTypeId;

        
        private readonly AppPermissionMode _appPermissionMode;
        private readonly string _appPermissionTypeId;




        //private readonly ActionPermission _actionPermission;

        public IDFAllowUserAndAppAttribute(/*string userRoleId,*/ string userEntityId, string userPermissionTypeId, AppPermissionMode appPermissionMode,string appPermissionTypeId) =>
            (_userEntityId, _userPermissionTypeId, _appPermissionMode, _appPermissionTypeId) = 
            (userEntityId, userPermissionTypeId, appPermissionMode,appPermissionTypeId);

        //public IDFAllowUserAttribute(ActionPermission actionPermission) =>
        //    (_actionPermission) = (actionPermission);

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Request.Headers.TryGetValue("userToken", out var userToken);
            context.HttpContext.Request.Headers.TryGetValue("appKey", out var appKey);
            context.HttpContext.Request.Headers.TryGetValue("appSecret", out var appSecret);

            if (string.IsNullOrEmpty(userToken) && (string.IsNullOrEmpty(appKey) || string.IsNullOrEmpty(appSecret)))
            {
                ReturnSecurityFail(context, SecurityValidationResult.IncorrectParameters.ToString());
            }

            SecurityValidationResult security = Helpers.ValidateUserTokenOrAppAccess(userToken.ToString(), _userEntityId, _userPermissionTypeId, appKey, appSecret, _appPermissionTypeId);


            if (security != SecurityValidationResult.Ok)
            {
                if (!string.IsNullOrEmpty(userToken))
                {
                    context.ActionArguments["requesterInfo"] = "UserId:" + IDFManager.UserTokenServices.GetUserIdUsingUsertoken(userToken);
                }
                if (!string.IsNullOrEmpty(appKey) && !string.IsNullOrEmpty(appKey))
                {
                    context.ActionArguments["requesterInfo"] = "AppId:" + IDFManager.UserTokenServices.Validate(userToken).UserId;
                }
                ReturnSecurityFail(context, security.ToString());
                return;
            }

            base.OnActionExecuting(context);
        }

        public void ReturnSecurityFail(ActionExecutingContext context, string content)
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

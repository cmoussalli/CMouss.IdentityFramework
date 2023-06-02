//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CMouss.IdentityFramework.API.Models;
//using CMouss.IdentityFramework.Services;
//using Newtonsoft.Json;

//namespace CMouss.IdentityFramework.API.Serving
//{



//    public class AllowUserAttribute : IDFBaseActionFilterAttribute
//    {

//        private readonly string _userEntityId;
//        private readonly string _userPermissionTypeId;



//        public override void OnActionExecuting(ActionExecutingContext context)
//        {
//            context.HttpContext.Request.Headers.TryGetValue("userToken", out var userToken);
//            //context.HttpContext.Request.Headers.TryGetValue("appKey", out var appKey);
//            //context.HttpContext.Request.Headers.TryGetValue("appSecret", out var appSecret);

//            if (string.IsNullOrEmpty(userToken))
//            {
//                Helpers.ReturnSecurityFail(context, SecurityValidationResult.IncorrectParameters.ToString());
//            }

//            AuthService auth = new();
//            AuthResult authResult = IDFManager.AuthService.AuthUserTokenWithPermission(userToken.ToString(), _userEntityId, _userPermissionTypeId);

//            if (authResult.SecurityValidationResult == SecurityValidationResult.Ok)
//            {
//                context.ActionArguments["requesterInfo"] = JsonConvert.SerializeObject(authResult);
//            }

//            base.OnActionExecuting(context);
//        }




//    }
//}

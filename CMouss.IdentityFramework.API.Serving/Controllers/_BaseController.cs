using CMouss.IdentityFramework.API.Models;
using CMouss.IdentityFramework.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CMouss.IdentityFramework;
using Microsoft.AspNetCore.Mvc.Filters;


namespace CMouss.IdentityFramework.API.Serving
{
    public class IDFBaseController : Controller
    {
        public UserClaim GetUserClaim()
        {
            UserClaim claim = new();
            bool canGetClaim = Request.Headers.TryGetValue("userToken", out var encryptedStr);
            if (!canGetClaim)
            {
                throw new Exception("UserToken is not found in the request header.");
            }
            claim = Helpers.DecryptUserToken(encryptedStr);
            return claim;
        }


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



        #region Security Validations
        public enum SecurityValidationResult
        {
            Ok = 0,
            UnknownError = 1,
            IncorrectParameters = 6,

            IncorrectToken = 11,
            IncorrectAppAccess = 12,
            UnAuthorized = 16,

        }

        #endregion


        #region Generic Responses
        public enum ResponseTemplate
        {
            Ok = 0
                , Exception = 1

                , NotFound = 10001
                , Duplicate = 10002
                , IncorrectParameters = 10003

                , IncorrectToken = 30010
                , IncorrectAppAccess = 30011
                , UnAuthenticated = 30016
                , UnAuthorized = 30017
        }


        public GenericResponseModel GetGenericResponse(ResponseTemplate template, string message, string error)
        {
            GenericResponseModel res = new();
            switch (template)
            {
                case ResponseTemplate.Ok: res.ResponseStatus.SetAsSuccess(message); break;
                //case ResponseTemplate.UnAuthorized: res.ResponseStatus.SetAsUnAuthorized(); break;
                //case ResponseTemplate.UnAuthenticated: res.ResponseStatus.(); break;

                default:
                    res.ResponseStatus.IsSuccess = false;
                    res.ResponseStatus.Message = message;
                    res.ResponseStatus.Errors.Add(new ErrorModel { Code = template.GetHashCode().ToString(), Message = error });
                    break;
            }
            return res;
        }
        public GenericResponseModel GetGenericResponse_Ok(string message)
        {
            GenericResponseModel res = new();
            res.ResponseStatus.IsSuccess = true;
            res.ResponseStatus.Message = message;
            return res;
        }
        public GenericResponseModel GetGenericResponse_Error(string error)
        {
            GenericResponseModel res = new();
            res.ResponseStatus.IsSuccess = false;
            res.ResponseStatus.Message = "";
            res.ResponseStatus.Errors.Add(new ErrorModel { Code = "", Message = error }); ;
            return res;
        }

        public GenericResponseModel GetGenericResponse_Error(int errorTypeId)
        {
            GenericResponseModel res = new();
            res.ResponseStatus.IsSuccess = false;
            res.ResponseStatus.Message = "";
            res.ResponseStatus.Errors.Add(new ErrorModel { Code = errorTypeId.ToString() });
            return res;
        }



        #endregion

    }
}

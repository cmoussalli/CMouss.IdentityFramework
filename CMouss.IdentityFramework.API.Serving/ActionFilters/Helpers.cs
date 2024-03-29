﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.API.Serving
{

    ///// <summary>
    ///// Set The authorization rules
    ///// </summary>
    //public class ActionPermission
    //{
    //    /// <summary>
    //    /// Specify the required granted entity access to user  
    //    /// </summary>
    //    public string EntityId { get; set; }

    //    /// <summary>
    //    /// Specify the required granted entity permission type access to user  
    //    /// </summary>
    //    public string PermissionTypeId { get; set; }

    //    /// <summary>
    //    /// Specify the required app permission type, used only if AppPermissionMode: SimpleMode & TenantMode
    //    /// </summary>
    //    public string AppPermissionTypeId { get; set; }

    //    /// <summary>
    //    /// Specify how you want app authorization behave.
    //    /// NotAllowed: Ignore app permissions.
    //    /// SimpleMode: Validate app permission type only, IGNORE user entity and permission type rules.
    //    /// Tenant Mode: Validate BOTH app permission type and user entity and permission type rules.
    //    /// </summary>
    //    public AppPermissionMode AppPermissionMode { get; set; } = AppPermissionMode.NotAllowed;

    //    public ActionPermission()
    //    {
    //        EntityId = "";
    //        PermissionTypeId = "";
    //        AppPermissionTypeId = "";
    //    }

    //}


    //public static class ActionPermissionFactory
    //{
    //    public static ActionPermission Create()
    //    {
    //        return new ActionPermission();
    //    }
    //}

    //public class RequesterInfo
    //{
    //    public IDFAuthenticationMode AuthenticationMode { get; set; }
    //}

    //public enum ResponseTemplate
    //{
    //    Ok = 0
    //    , Exception = 1

    //    , NotFound = 10001
    //    , Duplicate = 10002
    //    , IncorrectParameters = 10003

    //    , IncorrectToken = 30010
    //    , IncorrectAppAccess = 30011
    //    , UnAuthenticated = 30016
    //    , UnAuthorized = 30017


    //}






    public static class Helpers
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

        //public static SecurityValidationResult ValidateUserTokenOrAppAccess(string userToken, string userRole,
        //    string appAccessKey, string appAccessSecret, string appPermissionTypeId)
        //{
        //    SecurityValidationResult result = SecurityValidationResult.UnknownError;
        //    bool isAuthorized = false;
        //    if (
        //        (
        //        !String.IsNullOrEmpty(userToken) && !String.IsNullOrEmpty(userRole)
        //        && String.IsNullOrEmpty(appAccessKey) && String.IsNullOrEmpty(appAccessSecret) && String.IsNullOrEmpty(appPermissionTypeId)
        //        ) || (
        //        String.IsNullOrEmpty(userToken) && String.IsNullOrEmpty(userRole)
        //        && !String.IsNullOrEmpty(appAccessKey) && !String.IsNullOrEmpty(appAccessSecret) && !String.IsNullOrEmpty(appPermissionTypeId)
        //        )
        //    ) { }
        //    else { }

        //    try
        //    {
        //        if (!String.IsNullOrEmpty(userToken))
        //        { isAuthorized = IDFManager.UserServices.ValidateTokenRole(userToken, userRole); }

        //        if (!String.IsNullOrEmpty(appAccessKey) && isAuthorized == false)
        //        { isAuthorized = IDFManager.AppAccessServices.ValidateAppAccessPermission(appAccessKey, appAccessSecret, appPermissionTypeId); }

        //    }
        //    catch (InvalidAppAccessKeyOrSecretException)
        //    {
        //        result = SecurityValidationResult.IncorrectAppAccess;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {

        //        //switch (ex.Message)
        //        //{
        //        //    case "IncorrectToken":
        //        result = SecurityValidationResult.IncorrectToken;
        //        return result;
        //        //    break;

        //        //default:
        //        //    throw new Exception(ex.Message);
        //        //    break;
        //        //}


        //    }
        //    if (isAuthorized)
        //    {
        //        result = SecurityValidationResult.Ok;
        //    }
        //    else
        //    {
        //        result = SecurityValidationResult.UnAuthorized;
        //    }
        //    return result;
        //}

        //public static SecurityValidationResult ValidateUserTokenOrAppAccess(string userToken, string entityId, string permissionTypeId,
        //    string appAccessKey, string appAccessSecret, string appPermissionTypeId)
        //{
        //    SecurityValidationResult result = SecurityValidationResult.UnknownError;
        //    bool isAuthorized = false;
        //    if (
        //        (
        //        !String.IsNullOrEmpty(userToken) && !String.IsNullOrEmpty(entityId) && !String.IsNullOrEmpty(permissionTypeId)
        //        && String.IsNullOrEmpty(appAccessKey) && String.IsNullOrEmpty(appAccessSecret) && String.IsNullOrEmpty(appPermissionTypeId)
        //        ) || (
        //        String.IsNullOrEmpty(userToken) && String.IsNullOrEmpty(entityId) && String.IsNullOrEmpty(permissionTypeId)
        //        && !String.IsNullOrEmpty(appAccessKey) && !String.IsNullOrEmpty(appAccessSecret) && !String.IsNullOrEmpty(appPermissionTypeId)
        //        )
        //    ) { }
        //    else { }

        //    try
        //    {
        //        if (!String.IsNullOrEmpty(userToken))
        //        { isAuthorized = IDFManager.UserServices.ValidateTokenPermission(userToken, entityId, permissionTypeId); }

        //        if (!String.IsNullOrEmpty(appAccessKey) && !String.IsNullOrEmpty(appAccessSecret) && isAuthorized == false)
        //        { isAuthorized = IDFManager.AppAccessServices.ValidateAppAccessPermission(appAccessKey, appAccessSecret, appPermissionTypeId); }

        //    }
        //    catch (InvalidAppAccessKeyOrSecretException)
        //    {
        //        result = SecurityValidationResult.IncorrectAppAccess;
        //        return result;
        //    }
        //    catch (InvalidTokenException)
        //    {
        //        result = SecurityValidationResult.IncorrectToken;
        //        return result;
        //    }


        //    if (isAuthorized)
        //    {
        //        result = SecurityValidationResult.Ok;
        //    }
        //    else
        //    {
        //        result = SecurityValidationResult.UnAuthorized;
        //    }
        //    return result;
        //}




        #region Generic Responses



        //public GenericResponseModel GetGenericResponse(ResponseTemplate template, string message, string error)
        //{
        //    GenericResponseModel res = new();
        //    switch (template)
        //    {
        //        case ResponseTemplate.Ok: res.ResponseStatus.SetAsSuccess(message); break;
        //        //case ResponseTemplate.UnAuthorized: res.ResponseStatus.SetAsUnAuthorized(); break;
        //        //case ResponseTemplate.UnAuthenticated: res.ResponseStatus.(); break;

        //        default:
        //            res.ResponseStatus.IsSuccess = false;
        //            res.ResponseStatus.Message = message;
        //            res.ResponseStatus.Errors.Add(new ErrorModel { Code = template.GetHashCode().ToString(), Message = error });
        //            break;
        //    }
        //    return res;
        //}
        //public GenericResponseModel GetGenericResponse_Ok(string message)
        //{
        //    GenericResponseModel res = new();
        //    res.ResponseStatus.IsSuccess = true;
        //    res.ResponseStatus.Message = message;
        //    return res;
        //}
        //public GenericResponseModel GetGenericResponse_Error(string error)
        //{
        //    GenericResponseModel res = new();
        //    res.ResponseStatus.IsSuccess = false;
        //    res.ResponseStatus.Message = "";
        //    res.ResponseStatus.Errors.Add(new ErrorModel { Code = "", Message = error }); ;
        //    return res;
        //}

        //public GenericResponseModel GetGenericResponse_Error(int errorTypeId)
        //{
        //    GenericResponseModel res = new();
        //    res.ResponseStatus.IsSuccess = false;
        //    res.ResponseStatus.Message = "";
        //    res.ResponseStatus.Errors.Add(new ErrorModel { Code = errorTypeId.ToString() });
        //    return res;
        //}

        #endregion
    }
}

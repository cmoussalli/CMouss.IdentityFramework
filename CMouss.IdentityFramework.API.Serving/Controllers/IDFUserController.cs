﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using CMouss.IdentityFramework.API.Models;
using CMouss.IdentityFramework.API.Serving;
using Newtonsoft.Json;

namespace CMouss.IdentityFramework.API.Serving
{
    public class IDFUserController : Controller
    {

        #region Register
        [HttpPost]
        [Route(APIRoutes.User.Register)]
        public IActionResult Register(
            [FromHeader] string newid
          , [FromHeader] string username
          , [FromHeader] string password
          , [FromHeader] string fullname
          , [FromHeader] string email)
        {
            GenericResponseModel result = new();
            try
            {
                if (newid is null)
                {
                    newid = Guid.NewGuid().ToString();
                }
                IDFManager.userService.Register(username, password, fullname, email);
                result.ResponseStatus.SetAsSuccess();

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
            }
            return StatusCode(400, result);
        }
        #endregion


        #region Search
        [HttpPost]
        [Route(APIRoutes.User.Search)]
        [IDFAuthUserWithRolesOrPermissions("Administrators", "User:Search")]
        public IActionResult Search(
            [FromBody] IDFUserRequestModels.Search model
           , [FromHeader] string userToken
            , string requesterAuthInfo
       )
        {
            IDFUserResponseModels.Search result = new();
            try
            {
                if (model == null)
                {
                    model = new IDFUserRequestModels.Search();
                    model.UsersSearch = new Models.UsersSearch { UserName = "" };

                };
                List<CMouss.IdentityFramework.User> dbUsers = IDFManager.userService.Search(Converters.PagingConverter.ToDBPaging(model.Paging), Converters.UsersSearchConverter.ToDBUsersSearch(model.UsersSearch));
                result.ResponseStatus.SetAsSuccess();
                result.Users = Converters.UserConverter.ToAPIUsersList(dbUsers);
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }
            return Ok(result);
        }
        #endregion

        #region Details
        [HttpPost]
        [Route(APIRoutes.User.Details)]
        [IDFAuthUserWithRolesOrPermissions("Administrators", "User:Details")]
        public IActionResult Details(
            [FromBody] Models.IDFUserRequestModels.Details model
           , [FromHeader] string userToken
            , string requesterAuthInfo
       )
        {
            IDFUserResponseModels.Details result = new();
            try
            {
                result.ResponseStatus.SetAsSuccess();
                result.User = Converters.UserConverter.ToAPIUser(IDFManager.userService.Details(model.UserId), true, true, null);
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }
            return Ok(result);
        }
        #endregion

        #region Create
        [HttpPost]
        [Route(APIRoutes.User.Create)]
        [IDFAuthUserWithRolesOrPermissions("Administrators", "User:Create")]
        public IActionResult Create(
            [FromBody] Models.IDFUserRequestModels.Create model
           , [FromHeader] string userToken
            , string requesterAuthInfo)
        {
            GenericResponseModel result = new();
            try
            {

                string res = IDFManager.userService.Create(model.UserName, model.Password, model.FullName, model.Email, model.IsLocked, model.IsActive);
                result.ResponseStatus.SetAsSuccess("",res);
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
            }
            return StatusCode(400, result);
        }
        #endregion

        #region Update
        [HttpPost]
        [Route(APIRoutes.User.Update)]
        [IDFAuthUserWithRolesOrPermissions("Administrators", "User:Search")]
        public IActionResult Update(
            [FromBody] Models.IDFUserRequestModels.Update model
           , [FromHeader] string userToken
           , string requesterAuthInfo
       )
        {
            GenericResponseModel result = new();
            try
            {
                IDFManager.userService.Update(model.UserId, model.FullName, model.Email, model.IsLocked, model.IsActive );
                result.ResponseStatus.SetAsSuccess("", model.UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }

        }
        #endregion

        #region Delete
        [HttpPost]
        [Route(APIRoutes.User.Delete)]
        [IDFAuthUserWithRolesOrPermissions("Administrators", "User:Search")]
        public IActionResult Delete(
            [FromBody] Models.IDFUserRequestModels.Delete model
           , [FromHeader] string userToken
           , string requesterAuthInfo
       )
        {
            GenericResponseModel result = new();
            try
            {
                IDFManager.userService.Delete(model.UserId);
                result.ResponseStatus.SetAsSuccess("", model.UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }

        }
        #endregion





        #region Change Password
        [HttpPost]
        [Route(APIRoutes.User.ChangePassword)]
        [IDFAuthUserWithRolesOrPermissions("Administrators", "User:Search")]
        public IActionResult ChangePassword(
            [FromBody] Models.IDFUserRequestModels.ChangePassword model
           , [FromHeader] string userToken
            , string requesterAuthInfo
       )
        {
            GenericResponseModel result = new();
            try
            {
                IDFManager.userService.ChangePassword(model.UserId, model.NewPassword, model.ChangePrivateKey);
                result.ResponseStatus.SetAsSuccess();
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }

            return Ok(result);
        }
        #endregion

        #region Change My Password
        [HttpPost]
        [Route(APIRoutes.User.ChangeMyPassword)]
        public IActionResult ChangeMyPassword(
            [FromBody] Models.IDFUserRequestModels.ChangeMyPassword model
           , [FromHeader] string userToken
            , string requesterAuthInfo
       )
        {
            GenericResponseModel result = new();
            try
            {
            AuthResult authResult = JsonConvert.DeserializeObject<AuthResult>(requesterAuthInfo);
                IDFManager.userService.ChangePassword(authResult.UserToken.UserId, model.NewPassword, model.ChangePrivateKey);
                result.ResponseStatus.SetAsSuccess();
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }

            return Ok();
        }
        #endregion


        #region Lock
        [HttpPost]
        [Route(APIRoutes.User.Lock)]
        [IDFAuthUserWithRolesOrPermissions("Administrators", "User:Details")]
        public IActionResult Lock(
            [FromBody] Models.IDFUserRequestModels.Lock model
           , [FromHeader] string userToken
            , string requesterAuthInfo
       )
        {
            GenericResponseModel result = new();
            try
            {
                IDFManager.userService.Lock(model.UserId);
                result.ResponseStatus.SetAsSuccess();
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }

            return Ok();
        }
        #endregion

        #region Unlock
        [HttpPost]
        [Route(APIRoutes.User.Lock)]
        [IDFAuthUserWithRolesOrPermissions("Administrators", "User:Search")]
        public IActionResult Unlock(
            [FromBody] Models.IDFUserRequestModels.Lock model
           , [FromHeader] string userToken
            , string requesterAuthInfo
       )
        {
            GenericResponseModel result = new();
            try
            {
                IDFManager.userService.UnLock(model.UserId);
                result.ResponseStatus.SetAsSuccess();
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }

            return Ok();
        }
        #endregion


        #region Get Roles
        [HttpPost]
        [Route(APIRoutes.User.GetRoles)]
        [IDFAuthUserWithRolesOrPermissions("Administrators", "User:Create")]
        public IActionResult GetRoles(
            [FromBody] Models.IDFUserRequestModels.GetRoles model
           , [FromHeader] string userToken
            , string requesterAuthInfo
       )
        {
            IDFUserResponseModels.GetRoles result = new();
            try
            {
                List<API.Models.Role> apiRoles = Converters.RoleConverter.ToAPIRolesList(IDFManager.userService.GetRoles(model.UserId), false, false);
                result.ResponseStatus.SetAsSuccess();
                result.Roles = apiRoles;
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }
            return Ok(result);
        }
        #endregion

        #region Grant Role
        [HttpPost]
        [Route(APIRoutes.User.GrantRole)]
        [IDFAuthUserWithRolesOrPermissions("Administrators", "User:Search")]
        public IActionResult GrantRole(
            [FromBody] Models.IDFUserRequestModels.GrantRole model
           , [FromHeader] string userToken
            , string requesterAuthInfo
       )
        {
            GenericResponseModel result = new();
            try
            {
                IDFManager.userService.GrantRole(model.UserId, model.RoleId);
                result.ResponseStatus.SetAsSuccess();
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }

            return Ok(result);
        }
        #endregion

        #region Revoke Role
        [HttpPost]
        [Route(APIRoutes.User.RevokeRole)]
        [IDFAuthUserWithRolesOrPermissions("Administrators", "User:Search")]
        public IActionResult RevokeRole(
            [FromBody] Models.IDFUserRequestModels.RevokeRole model
           , [FromHeader] string userToken
            , string requesterAuthInfo
       )
        {
            GenericResponseModel result = new();
            try
            {
                IDFManager.userService.RevokeRole(model.UserId, model.RoleId);
                result.ResponseStatus.SetAsSuccess();
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }

            return Ok(result);
        }
        #endregion


        #region Validate User Role
        [HttpPost]
        [Route(APIRoutes.User.ValidateUserRole)]
        [IDFAuthUserWithRolesOrPermissions("Administrators", "User:Search")]
        public IActionResult ValidateUserRole(
            [FromBody] Models.IDFUserRequestModels.ValidateUserRole model
           , [FromHeader] string userToken
            , string requesterAuthInfo
       )
        {
            BooleanResponseModel result = new();
            try
            {
                bool res = IDFManager.userService.ValidateUserRole(model.UserId, model.RoleId);
                result.ResponseStatus.SetAsSuccess();
                result.Result = res;
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }

            return Ok(result);
        }
        #endregion

        #region Validate User Any Role
        [HttpPost]
        [Route(APIRoutes.User.ValidateUserAnyRole)]
        [IDFAuthUserWithRolesOrPermissions("Administrators", "User:Details")]
        public IActionResult ValidateUserAnyRole(
            [FromBody] Models.IDFUserRequestModels.ValidateUserAnyRole model
           , [FromHeader] string userToken
            , string requesterAuthInfo
       )
        {
            BooleanResponseModel result = new();
            try
            {
                bool res = IDFManager.userService.ValidateUserRole(model.UserId, model.RoleIds);
                result.ResponseStatus.SetAsSuccess();
                result.Result = res;
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }

            return Ok(result);
        }
        #endregion


        #region Validate Token Role
        [HttpPost]
        [Route(APIRoutes.User.ValidateTokenRole)]
        [IDFAuthUserWithRolesOrPermissions("Administrators", "User:Search")]
        public IActionResult ValidateTokenRole(
            [FromBody] Models.IDFUserRequestModels.ValidateTokenRole model
           , [FromHeader] string userToken
       )
        {
            BooleanResponseModel result = new();
            try
            {
                bool res = IDFManager.userService.ValidateUserRole(model.Token, model.RoleId);
                result.ResponseStatus.SetAsSuccess();
                result.Result = res;
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }

            return Ok(result);
        }
        #endregion

       // #region Validate Token Any Role
       // [HttpPost]
       // [Route(APIRoutes.User.ValidateTokenAnyRole)]
       // public IActionResult ValidateTokenAnyRole(
       //     [FromBody] Models.IDFUserRequestModels.ValidateTokenAnyRole model
       //    , [FromHeader] string userToken
       //)
       // {
       //     #region Validate Token
       //     UserToken token = IDFManager.UserTokenServices.Validate(userToken);
       //     if (userToken == null)
       //     {
       //         return Unauthorized(Messages.IncorrectToken);
       //     }

       //     if (!IDFManager.UserServices.ValidateUserRoleOrPermission(token.UserId, IDFManager.AdministratorRoleId, "Users", "ValidateTokenAnyRole"))
       //     {
       //         return Unauthorized();
       //     }

       //     #endregion

       //     BooleanResponseModel result = new();
       //     try
       //     {

       //         bool res = IDFManager.UserServices.(model.Token, model.RoleIds);
       //         result.ResponseStatus.SetAsSuccess();
       //         result.Result = res;
       //     }
       //     catch (Exception ex)
       //     {
       //         result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
       //         return StatusCode(400, result);
       //     }

       //     return Ok(result);
       // }
       // #endregion





    }
}

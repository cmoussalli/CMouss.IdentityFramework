using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using CMouss.IdentityFramework.API.Models;
using CMouss.IdentityFramework.API.Serving;

namespace CMouss.IdentityFramework.API.Serving.Controllers
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
            GenericResponseModel result = new(false);
            try
            {

                if (newid is null)
                {
                    newid = Guid.NewGuid().ToString();
                }
                IDFManager.UserServices.Register(username, password, fullname, email);
                result = new GenericResponseModel(true);
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return StatusCode(400, result);
        }
        #endregion

        #region Login
        [HttpPost]
        [Route(APIRoutes.User.Login)]
        public IActionResult Login(
             [FromHeader] string username
            , [FromHeader] string password
        )
        {
            API.Models.UserToken result = new();
            try
            {
                UserToken dbUserToken = IDFManager.UserServices.UserLogin(username, password);
                if (!String.IsNullOrEmpty(dbUserToken.Token))
                {
                    return Ok(Converters.UserTokenConverter.ToAPIUserToken(dbUserToken));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NotFound();
        }
        #endregion

        #region Validate UserTooken
        [HttpPost]
        [Route(APIRoutes.User.ValidateToken)]
        public IActionResult ValidateToken(
             [FromHeader] string token
        )
        {
            API.Models.UserToken result = new();
            try
            {
                UserToken dbUserToken = IDFManager.UserTokenServices.Validate(token);
                if (dbUserToken is not null)
                {
                    return Ok(Converters.UserTokenConverter.ToAPIUserToken(dbUserToken));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NotFound();
        }
        #endregion


        #region Search
        [HttpPost]
        [Route(APIRoutes.User.Search)]
        public IActionResult Search(
            [FromBody] IDFUserRequestModels.Search model
           , [FromHeader] string token
       )
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized(Messages.IncorrectToken);
            }

            if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "Search"))
            {
                return Unauthorized();
            }

            #endregion


            List<API.Models.User> result = new();
            try
            {
                if (model == null)
                {
                    model = new IDFUserRequestModels.Search();
                    model.UsersSearch = new Models.UsersSearch { UserName = "" };

                };
                List<CMouss.IdentityFramework.User> dbUsers = IDFManager.UserServices.Search(Converters.PagingConverter.ToDBPaging(model.Paging), Converters.UsersSearchConverter.ToDBUsersSearch(model.UsersSearch));
                result = Converters.UserConverter.ToAPIUsersList(dbUsers);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            return Ok(result);
        }
        #endregion

        #region Details
        [HttpPost]
        [Route(APIRoutes.User.Details)]
        public IActionResult Details(
            [FromBody] Models.IDFUserRequestModels.Details model
           , [FromHeader] string token
       )
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized();
            }
            if (userToken.UserId.ToLower() != model.UserId.ToLower())
            {
                if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "Details"))
                {
                    return Unauthorized();
                }
            }
            #endregion

            API.Models.User result = new();
            try
            {
                result = Converters.UserConverter.ToAPIUser(IDFManager.UserServices.Details(model.UserId), true, true, null);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            return Ok(result);
        }
        #endregion

        #region Create
        [HttpPost]
        [Route(APIRoutes.User.Create)]
        public IActionResult Create(
            [FromBody] Models.IDFUserRequestModels.Create model
           , [FromHeader] string token)
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized(Messages.IncorrectToken);
            }

            if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "Create"))
            {
                return Unauthorized();
            }

            #endregion

            GenericResponseModel result = new(false);
            try
            {

                string res = IDFManager.UserServices.Create(model.UserName, model.Password, model.FullName, model.Email, model.IsLocked, model.IsActive);
                result = new GenericResponseModel(true);
                result.ReferenceId = res;
                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return StatusCode(400, result);
        }
        #endregion

        #region Update
        [HttpPost]
        [Route(APIRoutes.User.Update)]
        public IActionResult Update(
            [FromBody] Models.IDFUserRequestModels.Update model
           , [FromHeader] string token
       )
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized(Messages.IncorrectToken);
            }
            if (userToken.UserId.ToLower() != model.UserId.ToLower())
            {
                if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "Update"))
                {
                    return Unauthorized();
                }
            }
            #endregion

            GenericResponseModel result = new(false);
            result.ReferenceId = model.UserId;
            try
            {
                IDFManager.UserServices.Update(model.UserId, model.FullName, model.Email, model.IsLocked, model.IsActive);
                return Ok(new GenericResponseModel(true));
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return StatusCode(400, result);
            }

        }
        #endregion

        #region Delete
        [HttpPost]
        [Route(APIRoutes.User.Delete)]
        public IActionResult Delete(
            [FromBody] Models.IDFUserRequestModels.Delete model
           , [FromHeader] string token
       )
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized(Messages.IncorrectToken);
            }
            if (userToken.UserId.ToLower() != model.UserId.ToLower())
            {
                if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "Delete"))
                {
                    return Unauthorized();
                }
            }
            #endregion

            GenericResponseModel result = new(false);
            result.ReferenceId = model.UserId;
            try
            {
                IDFManager.UserServices.Delete(model.UserId);
                return Ok(new GenericResponseModel(true));
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return StatusCode(400, result);
            }

        }
        #endregion








        #region Change Password
        [HttpPost]
        [Route(APIRoutes.User.ChangePassword)]
        public IActionResult ChangePassword(
            [FromBody] Models.IDFUserRequestModels.ChangePassword model
           , [FromHeader] string token
       )
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized();
            }
            if (userToken.UserId.ToLower() != model.UserId.ToLower())
            {
                if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "ChangePassword"))
                {
                    return Unauthorized();
                }
            }
            #endregion

            GenericResponseModel result = new(false);
            try
            {
                IDFManager.UserServices.ChangePassword(model.UserId, model.NewPassword, model.ChangePrivateKey);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return StatusCode(400, result);
            }

            return Ok();
        }
        #endregion

        #region Change My Password
        [HttpPost]
        [Route(APIRoutes.User.ChangeMyPassword)]
        public IActionResult ChangeMyPassword(
            [FromBody] Models.IDFUserRequestModels.ChangeMyPassword model
           , [FromHeader] string token
       )
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized();
            }
            if (userToken.UserId.ToLower() != model.UserId.ToLower())
            {
                return Unauthorized();
                //if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "ChangePassword"))
                //{
                //    return Unauthorized();
                //}
            }
            #endregion

            GenericResponseModel result = new(false);
            try
            {
                IDFManager.UserServices.ChangePassword(model.UserId, model.NewPassword, model.ChangePrivateKey);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return StatusCode(400, result);
            }

            return Ok();
        }
        #endregion


        #region Lock
        [HttpPost]
        [Route(APIRoutes.User.Lock)]
        public IActionResult Lock(
            [FromBody] Models.IDFUserRequestModels.Lock model
           , [FromHeader] string token
       )
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized(Messages.IncorrectToken);
            }
            if (userToken.UserId.ToLower() != model.UserId.ToLower())
            {
                if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "Lock"))
                {
                    return Unauthorized();
                }
            }
            #endregion

            GenericResponseModel result = new(false);
            try
            {
                IDFManager.UserServices.Lock(model.UserId);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return StatusCode(400, result);
            }

            return Ok();
        }
        #endregion

        #region Unlock
        [HttpPost]
        [Route(APIRoutes.User.Lock)]
        public IActionResult Unlock(
            [FromBody] Models.IDFUserRequestModels.Lock model
           , [FromHeader] string token
       )
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized(Messages.IncorrectToken);
            }
            if (userToken.UserId.ToLower() != model.UserId.ToLower())
            {
                if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "Unlock"))
                {
                    return Unauthorized();
                }
            }
            #endregion

            GenericResponseModel result = new(false);
            try
            {
                IDFManager.UserServices.UnLock(model.UserId);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return StatusCode(400, result);
            }

            return Ok();
        }
        #endregion


        #region Get Roles
        [HttpPost]
        [Route(APIRoutes.User.GetRoles)]
        public IActionResult GetRoles(
            [FromBody] Models.IDFUserRequestModels.GetRoles model
           , [FromHeader] string token
       )
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized(Messages.IncorrectToken);
            }

            if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "GetRoles"))
            {
                return Unauthorized();
            }

            #endregion

            GenericResponseModel result = new(false);
            try
            {
                List<API.Models.Role> apiRoles = Converters.RoleConverter.ToAPIRolesList(IDFManager.UserServices.GetRoles(model.UserId), false, false);

                return Ok(apiRoles);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return StatusCode(400, result);
            }

            return Ok();
        }
        #endregion

        #region Grant Role
        [HttpPost]
        [Route(APIRoutes.User.GrantRole)]
        public IActionResult GrantRole(
            [FromBody] Models.IDFUserRequestModels.GrantRole model
           , [FromHeader] string token
       )
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized(Messages.IncorrectToken);
            }

            if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "GrantRole"))
            {
                return Unauthorized();
            }

            #endregion

            GenericResponseModel result = new(false);
            try
            {
                IDFManager.UserServices.GrantRole(model.UserId, model.RoleId);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return StatusCode(400, result);
            }

            return Ok();
        }
        #endregion

        #region Revoke Role
        [HttpPost]
        [Route(APIRoutes.User.RevokeRole)]
        public IActionResult RevokeRole(
            [FromBody] Models.IDFUserRequestModels.RevokeRole model
           , [FromHeader] string token
       )
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized(Messages.IncorrectToken);
            }

            if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "RevokeRole"))
            {
                return Unauthorized();
            }

            #endregion

            GenericResponseModel result = new(false);
            try
            {
                IDFManager.UserServices.RevokeRole(model.UserId, model.RoleId);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return StatusCode(400, result);
            }

            return Ok();
        }
        #endregion


        #region Validate User Role
        [HttpPost]
        [Route(APIRoutes.User.ValidateUserRole)]
        public IActionResult ValidateUserRole(
            [FromBody] Models.IDFUserRequestModels.ValidateUserRole model
           , [FromHeader] string token
       )
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized(Messages.IncorrectToken);
            }

            if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "ValidateUserRole"))
            {
                return Unauthorized();
            }

            #endregion

            GenericResponseModel result = new(false);
            try
            {
                
                bool res = IDFManager.UserServices.ValidateUserRole(model.UserId, model.RoleId);
                result = new(true,res.ToString().ToLower());
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return StatusCode(400, result);
            }

            return Ok(result);
        }
        #endregion

        #region Validate User Any Role
        [HttpPost]
        [Route(APIRoutes.User.ValidateUserAnyRole)]
        public IActionResult ValidateUserAnyRole(
            [FromBody] Models.IDFUserRequestModels.ValidateUserAnyRole model
           , [FromHeader] string token
       )
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized(Messages.IncorrectToken);
            }

            if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "ValidateUserAnyRole"))
            {
                return Unauthorized();
            }

            #endregion

            GenericResponseModel result = new(false);
            try
            {

                bool res = IDFManager.UserServices.ValidateUserRole(model.UserId, model.RoleIds);
                result = new(true, res.ToString().ToLower());
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return StatusCode(400, result);
            }

            return Ok(result);
        }
        #endregion


        #region Validate Token Role
        [HttpPost]
        [Route(APIRoutes.User.ValidateTokenRole)]
        public IActionResult ValidateTokenRole(
            [FromBody] Models.IDFUserRequestModels.ValidateTokenRole model
           , [FromHeader] string token
       )
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized(Messages.IncorrectToken);
            }

            if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "ValidateTokenRole"))
            {
                return Unauthorized();
            }

            #endregion

            GenericResponseModel result = new(false);
            try
            {

                bool res = IDFManager.UserServices.ValidateTokenRole(model.Token, model.RoleId);
                result = new(true, res.ToString().ToLower());
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return StatusCode(400, result);
            }

            return Ok(result);
        }
        #endregion

        #region Validate Token Any Role
        [HttpPost]
        [Route(APIRoutes.User.ValidateTokenAnyRole)]
        public IActionResult ValidateTokenAnyRole(
            [FromBody] Models.IDFUserRequestModels.ValidateTokenAnyRole model
           , [FromHeader] string token
       )
        {
            #region Validate Token
            UserToken userToken = IDFManager.UserTokenServices.Validate(token);
            if (userToken == null)
            {
                return Unauthorized(Messages.IncorrectToken);
            }

            if (!IDFManager.UserServices.ValidateUserRoleOrPermission(userToken.UserId, IDFManager.AdministratorRoleId, "Users", "ValidateTokenAnyRole"))
            {
                return Unauthorized();
            }

            #endregion

            GenericResponseModel result = new(false);
            try
            {

                bool res = IDFManager.UserServices.ValidateTokenRole(model.Token, model.RoleIds);
                result = new(true, res.ToString().ToLower());
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return StatusCode(400, result);
            }

            return Ok(result);
        }
        #endregion





    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using CMouss.IdentityFramework.API.Models;
using CMouss.IdentityFramework.API.Serving;

namespace CMouss.IdentityFramework.API.Serving
{
    public class IDFAuthController : Controller
    {

        #region Login
        [HttpPost]
        [Route(APIRoutes.User.Login)]
        public IActionResult Login(
             [FromHeader] string username
            , [FromHeader] string password
        )
        {
            IDFUserResponseModels.Login result = new();
            try
            {
                UserToken dbUserToken = IDFManager.UserServices.UserLogin(username, password);
                if (!String.IsNullOrEmpty(dbUserToken.Token))
                {
                    result.ResponseStatus.SetAsSuccess();
                    result.UserToken = Converters.UserTokenConverter.ToAPIUserToken(dbUserToken);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }
            result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = "Incorrect token" });
            return StatusCode(400, result);
        }
        #endregion

        #region Validate UserTooken
        [HttpPost]
        [Route(APIRoutes.User.ValidateToken)]
        public IActionResult ValidateToken(
             [FromHeader] string token
        )
        {
            IDFUserResponseModels.Login result = new();
            try
            {
                UserToken dbUserToken = IDFManager.UserTokenServices.Validate(token);
                if (!String.IsNullOrEmpty(dbUserToken.Token))
                {
                    result.ResponseStatus.SetAsSuccess();
                    result.UserToken = Converters.UserTokenConverter.ToAPIUserToken(dbUserToken);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }
            result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = "Incorrect token" });
            return StatusCode(400, result);
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

            BooleanResponseModel result = new();
            try
            {
                
                bool res = IDFManager.UserServices.ValidateUserRole(model.UserId, model.RoleId);
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

      





    }
}

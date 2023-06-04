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
             [FromHeader] string userToken
        )
        {
            IDFUserResponseModels.Login result = new();
            try
            {
                UserToken dbUserToken = IDFManager.UserTokenServices.Validate(userToken);
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
           , [FromHeader] string userToken
       )
        {
            #region Validate Token
            UserToken token = IDFManager.UserTokenServices.Validate(userToken);
            if (userToken == null)
            {
                return Unauthorized(Messages.IncorrectToken);
            }

            if (!IDFManager.UserServices.ValidateUserRoleOrPermission(token.UserId, IDFManager.AdministratorRoleId, "Users", "ValidateUserRole"))
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

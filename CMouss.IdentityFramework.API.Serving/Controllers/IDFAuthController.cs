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
        [Route(APIRoutes.Auth.UserLogin)]
        
        public IActionResult UserLogin( 
             [FromHeader] string username
            , [FromHeader] string password
        )
        {
            IDFUserResponseModels.UserLogin result = new();
            try
            {
                UserToken dbUserToken = IDFManager.AuthService.UserLogin(username, password);
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

        #region Validate UserToken
        [HttpPost]
        
        [Route(APIRoutes.Auth.UserToken)]
        public IActionResult UserToken(
             [FromHeader] string userToken
        )
        {
            IDFUserResponseModels.UserLogin result = new();
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











    }
}

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
            IDFAuthResponseModels.IDFAuth result = new();
            try
            {
                AuthResult authResult = IDFManager.AuthService.AuthUserLogin(username, password);
                result.ResponseStatus.SetAsSuccess();
                result.AuthResult = Converters.AuthResultConverter.ToAPIAuthResult(authResult);

            }
            catch (Exception ex)
            {
                result.ResponseStatus.SetAsFailed(new ErrorModel() { Message = ex.Message });
                return StatusCode(400, result);
            }
            return Ok(result);
        }
        #endregion

        #region Validate UserToken
        [HttpPost]

        [Route(APIRoutes.Auth.UserToken)]
        public IActionResult UserToken(
             [FromHeader] string userToken
        )
        {
            IDFAuthResponseModels.IDFAuth result = new();
            try
            {
                AuthResult authResult = IDFManager.AuthService.AuthUserToken(userToken);
                result.ResponseStatus.SetAsSuccess();
                result.AuthResult = Converters.AuthResultConverter.ToAPIAuthResult(authResult);
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

using CMouss.IdentityFramework.API.Serving;
using CMouss.IdentityFramework.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.APIServer.Controllers
{
    public class TestController : IDFBaseController
    {

        [HttpGet]
        [Route("api/test")]
        public async Task<IActionResult> Test(
            [FromHeader] string userToken
            , string requesterAuthInfo)
        {
            return Ok(Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }



        //[HttpPost("api/test/secureUserAndApp")]
        //[HttpPost]
        //[Route(APIRoutes.Test.TestMain)]
        //[IDFSecure(userEntityId: "Entity1", userPermissionTypeId: "UserPermissionType1", appPermissionTypeId: "AppPermissionType1")]
        //public async Task<IActionResult> SecureUserAndApp([FromBody]
        //    [FromHeader] string userToken
        //    , [FromHeader] string appKey
        //    , [FromHeader] string appSecret
        //    , string requesterinfo)
        //{

        //    return Ok(requesterinfo);
        //}

        //[HttpPost]
        //[Route(APIRoutes.Test.TestMain)]
        //[IDFAllowUserAndApp("Entity1","UserPermissionType1",AppPermissionMode.SimpleMode, "AppPermissionType1")]
        //public IActionResult Test()
        //{
        //    return Ok("IdentityFramework is OK");
        //}


        [HttpPost]
        [Route("api/test/usertoken")]
        [IDFAuthUser()]
        public async Task<IActionResult> TestAuthUserToken(
            [FromHeader] string userToken)
        {
            //AuthResult r = JsonConvert.DeserializeObject<AuthResult>(requesterAuthInfo);
            //return Ok(ConvertAuthResultToShortString(r) + Environment.NewLine
            //    + "RequesterAuthInfo:" + Environment.NewLine + requesterAuthInfo);

            UserClaim claim = GetUserClaim();
            return Ok(claim);

        }

        [HttpPost]
        [Route("api/test/UserWithRole")]
        [IDFAuthUserWithRole("Administrators")]
        public async Task<IActionResult> TestAuthUserWithRole(
            [FromHeader] string userToken)
        {
            UserClaim claim = GetUserClaim();
            return Ok(claim);
        }

        [HttpPost]
        [Route("api/test/UserWithPermission")]
        [IDFAuthUserWithPermission("Entity1", "PermissionType1")]
        public async Task<IActionResult> TestAuthUserWithPermission(
            [FromHeader] string userToken
            , string requesterAuthInfo)
        {
            AuthResult r = JsonConvert.DeserializeObject<AuthResult>(requesterAuthInfo);
            return Ok(ConvertAuthResultToShortString(r) + Environment.NewLine
                + "RequesterAuthInfo:" + Environment.NewLine + requesterAuthInfo);
        }

        [HttpPost]
        [Route("api/test/UserWithRolesOrPermission")]
        [IDFAuthUserWithRoleOrPermission("Administrators", "Entity1", "PermissionType1")]
        public async Task<IActionResult> TestAuthUserWithRolesOrPermission(
            [FromHeader] string userToken
            , string requesterAuthInfo)
        {
            AuthResult r = JsonConvert.DeserializeObject<AuthResult>(requesterAuthInfo);
            return Ok(ConvertAuthResultToShortString(r) + Environment.NewLine
                + "RequesterAuthInfo:" + Environment.NewLine + requesterAuthInfo);
        }


        public string ConvertAuthResultToShortString(AuthResult authResult)
        {
            return $"Auth Test:{Environment.NewLine}AuthMode: {authResult.AuthenticationMode.ToString()}, UserId: {authResult.UserToken.User.Id}, UserName: {authResult.UserToken.User.UserName}";
        }
    }
}

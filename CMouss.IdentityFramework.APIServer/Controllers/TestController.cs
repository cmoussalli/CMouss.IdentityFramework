using CMouss.IdentityFramework.API.Serving;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CMouss.IdentityFramework.APIServer.Controllers
{
    public class TestController : Controller
    {

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

        [HttpPost]
        [Route(APIRoutes.Test.TestMain)]
        [IDFAllowUserAndApp("Entity1","UserPermissionType1",AppPermissionMode.SimpleMode, "AppPermissionType1")]
        public IActionResult Test()
        {
            return Ok("IdentityFramework is OK");
        }


    }
}

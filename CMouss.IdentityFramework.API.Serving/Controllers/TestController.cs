using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace CMouss.IdentityFramework.API.Serving
{
    public class TestController : Controller
    {

        [HttpPost]
        [Route(APIRoutes.Test.Echo)]
        public IActionResult Test()
        {
            return Ok("Echo test from CMouss.IdentityFramework, Client IPAddress: " + Request.HttpContext.Connection.RemoteIpAddress );
        }




    }
}

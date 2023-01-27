using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace CMouss.IdentityFramework.API.Serving.Controllers
{
    public class TestController : Controller
    {

        [HttpPost]
        [Route(APIRoutes.Test.TestMain)]
        public IActionResult Test()
        {
            return Ok("IdentityFramework is OK");
        }



    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpController : ControllerBase
    {
        [HttpGet("up")]
        public ActionResult GetUp()
        {
            return NoContent();
        }
    }
}

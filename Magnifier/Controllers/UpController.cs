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
        /// <summary>
        /// something stupid
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("up")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult GetUp()
        {
            return NoContent();
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public ActionResult AuthenticatedEndpoint()
        {
            return Ok("You are authenticated!");
        }
    }
}

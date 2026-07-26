using LibraryManagementClassLib.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ICommonService _commonService;

        public CommonController(ICommonService commonService)
        {
            _commonService = commonService;
        }

        [HttpGet("user-dashboard/{userId}")]
        public async Task<IActionResult> UserDashboard([FromRoute] int userId)
        {
            var result = await _commonService.UserDashboard(userId);
            return Ok(result);
        }

        [HttpGet("admin-dashboard")]
        public async Task<IActionResult> AdminDashboard()
        {
            var result = await _commonService.AdminDashboard();
            return Ok(result);
        }
    }
}

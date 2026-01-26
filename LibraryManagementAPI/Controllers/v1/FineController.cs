using Asp.Versioning;
using LibraryManagementClassLib.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class FineController : ControllerBase
    {
        private readonly IFineService _fineService;

        public FineController(IFineService fineService)
        {
            _fineService = fineService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetFine(int issueId)
        {
            var fine = await _fineService.GetFineAsync(issueId);
            return Ok(fine);
        }
    }
}

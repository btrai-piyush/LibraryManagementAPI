using Asp.Versioning;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize]
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

        [HttpPost("get-all")]
        public async Task<IActionResult> GetAllFines(GeneralQueryDto query)  
        {
            var fines = await _fineService.CalculateAllFines(query);
            return Ok(fines);
        }

        [HttpPost("user-fines")]
        public async Task<IActionResult> GetUserFines(UserFineQueryDto query)
        {
            try
            {
                var fines = await _fineService.GetUserFines(query);
                return Ok(fines);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("pay-fine")]
        public async Task<IActionResult> PayFine(int fineId)
        {
            try
            {
                var result = await _fineService.PayFineAsync(fineId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

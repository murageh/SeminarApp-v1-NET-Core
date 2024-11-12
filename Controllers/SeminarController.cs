using Microsoft.AspNetCore.Mvc;
using SeminarIntegration.Interfaces;
using SeminarIntegration.Services;

namespace SeminarIntegration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SeminarController (ISeminar seminar) : Controller
    {
        [HttpPatch("usingService")]
        public async Task<dynamic> PostDataToBc(SeminarData seminarData)
        {
            var response = await seminar.PostData(seminarData);
            return Ok(response);
        }
        
        [HttpPatch("usingOdata")]
        public async Task<dynamic> PostDataToBcViaOdata(SeminarData seminarData)
        {
            var page = "RenameSeminarV2";
            var response = await seminar.PostData(seminarData, page);
            return Ok(response);
        }
    }
}
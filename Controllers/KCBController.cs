using Microsoft.AspNetCore.Mvc;
using SeminarIntegration.Models;

namespace SeminarIntegration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class KcbController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetToken()
        {
            try
            {
                var kcb = await Connection.GetKCBToken();
                if (kcb == null) {
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Failed to get KCB token"
                    });
                }

                if (!kcb.success)
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        message = kcb.errorRes.error_description
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "KCB token fetched successfully",
                    token = kcb.successRes.access_token
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = e.Message
                });
            }
        }
    }
}

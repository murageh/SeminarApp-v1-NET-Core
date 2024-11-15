using Microsoft.AspNetCore.Mvc;
using SeminarIntegration.DTOs;
using SeminarIntegration.Interfaces;

namespace SeminarIntegration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AuthController(IAuthService authService) : Controller
    {
        private readonly ControllerHelpers _helpers = new();

        [HttpPost("register")]
        [ActionName("register")]
        [EndpointDescription("Registers a user into the system")]
        public async Task<IActionResult> Register(NewUserRequest newUserRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return await _helpers.HandleRequestAsync(
                () => authService.RegisterUser(newUserRequest),
                "Logging in",
                "User logged in successfully.",
                200,
                HttpContext.Request.Path
            );
        }
        
        [HttpPost("login")]
        [ActionName("login")]
        [EndpointDescription("Logs in a user")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return await _helpers.HandleRequestAsync(
                () => authService.AuthenticateUserAndReturnToken(loginRequest.Username, loginRequest.Password),
                "Logging in",
                "User logged in successfully.",
                200,
                HttpContext.Request.Path
            );
        }
    }
}

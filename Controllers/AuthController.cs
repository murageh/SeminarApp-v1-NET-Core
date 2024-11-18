using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeminarIntegration.DTOs;
using SeminarIntegration.Interfaces;
using System.IdentityModel.Tokens.Jwt;

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

        [HttpGet("profile")]
        [Authorize]
        [ActionName("profile")]
        [EndpointDescription("Gets the current user profile")]
        public async Task<IActionResult> GetProfile()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Invalid Authorization header.");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            var username = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Invalid token.");
            }

            return await _helpers.HandleRequestAsync(
                () => authService.GetUserProfile(username),
                "Finding user",
                "User found successfully.",
                200,
                HttpContext.Request.Path
            );
        }
    }
}

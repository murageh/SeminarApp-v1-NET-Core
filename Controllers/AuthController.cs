using System.IdentityModel.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeminarIntegration.DTOs;
using SeminarIntegration.DTOs.Authentication;
using SeminarIntegration.Interfaces;
using SeminarIntegration.Models;

namespace SeminarIntegration.Controllers;

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
            return StatusCode(
                StatusCodes.Status400BadRequest,
                new AppResponse<NormalUserResponse>.ErrorResponse()
                {
                    Title = "Registration",
                    Message = "Invalid request.",
                    Path = HttpContext.Request.Path,
                    StatusCode = (int)HttpStatusCode.BadRequest
                }
            );

        var res = await authService.RegisterUser(newUserRequest);
        res.Path = HttpContext.Request.Path;
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("login")]
    [ActionName("login")]
    [EndpointDescription("Logs in a user")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        if (!ModelState.IsValid)
            return StatusCode(
                StatusCodes.Status400BadRequest,
                new AppResponse<NormalUserResponse>.ErrorResponse()
                {
                    Title = "Login",
                    Message = "Invalid request.",
                    Path = HttpContext.Request.Path,
                    StatusCode = (int)HttpStatusCode.BadRequest
                }
            );

        var res = await authService.AuthenticateUserAndReturnToken(loginRequest.Username, loginRequest.Password);
        res.Path = HttpContext.Request.Path;
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("profile")]
    [Authorize]
    [ActionName("profile")]
    [EndpointDescription("Gets the current user profile")]
    public async Task<IActionResult> GetProfile()
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer "))
            return StatusCode(StatusCodes.Status401Unauthorized, new AppResponse<NormalUserResponse>.ErrorResponse()
            {
                Title = "Profile",
                Message = "Invalid token.",
                Path = HttpContext.Request.Path,
                StatusCode = (int)HttpStatusCode.Unauthorized
            });

        var token = authHeader.Substring("Bearer ".Length).Trim();
        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
        var username = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;

        if (string.IsNullOrEmpty(username))
            return StatusCode(
                StatusCodes.Status401Unauthorized,
                new AppResponse<NormalUserResponse>.ErrorResponse()
                {
                    Title = "Profile",
                    Message = "Invalid token.",
                    Path = HttpContext.Request.Path,
                    StatusCode = (int)HttpStatusCode.Unauthorized
                });

        var res = await authService.GetUserProfile(username);
        res.Path = HttpContext.Request.Path;
        return StatusCode(res.StatusCode, res);
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeminarIntegration.DTOs;
using SeminarIntegration.Interfaces;

namespace SeminarIntegration.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserController(IUserService userService) : Controller
{
    private readonly ControllerHelpers _helpers = new();

    // [HttpPost]
    // [ActionName("create")]
    // [EndpointDescription("Creates a new user")]
    // public async Task<IActionResult> CreateUser(NewUserRequest newUser)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest(ModelState);
    //     }
    //
    //     return await _helpers.HandleRequestAsync(
    //         () => userService.CreateUser(newUser),
    //         "Creating user",
    //         "User created successfully.",
    //         201,
    //         HttpContext.Request.Path
    //     );
    // }

    [HttpGet]
    [ActionName("all")]
    [EndpointDescription("Fetches all users")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetUsers()
    {
        var response = await userService.GetUsersAsync();
        response.Path = HttpContext.Request.Path;
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("all")]
    [ActionName("allUsers")]
    [EndpointDescription("Fetches all users, including deleted users")]
    [Authorize(Policy = "User")] // TODO: Change to Admin or elevated User
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await userService.GetAllUsersAsync();
        response.Path = HttpContext.Request.Path;
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{username}")]
    [ActionName("get")]
    [EndpointDescription("Fetches a specific user")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetUser(string username)
    {
        var response = await userService.GetUser(username);
        response.Path = HttpContext.Request.Path;
        return StatusCode(response.StatusCode, response);
    }

    [HttpPatch("{username}")]
    [ActionName("update")]
    [EndpointDescription("Updates an existing user")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> UpdateUser(string username, UpdateUserRequest updatedUser)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var response = await userService.UpdateUser(username, updatedUser);
        response.Path = HttpContext.Request.Path;
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{username}")]
    [ActionName("delete")]
    [EndpointDescription("Soft deletes a user")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> DeleteUser(string username)
    {
        var response = await userService.DeleteUser(username);
        response.Path = HttpContext.Request.Path;
        return StatusCode(response.StatusCode, response);
    }
}
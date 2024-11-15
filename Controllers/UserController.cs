using Microsoft.AspNetCore.Mvc;
using SeminarIntegration.DTOs;
using SeminarIntegration.Interfaces;
using SeminarIntegration.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace SeminarIntegration.Controllers
{
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
            return await _helpers.HandleRequestAsync(
                () => userService.GetUsersAsync(),
                "Finding users",
                "Users found successfully.",
                200,
                HttpContext.Request.Path
            );
        }

        [HttpGet("all")]
        [ActionName("allUsers")]
        [EndpointDescription("Fetches all users, including deleted users")]
        [Authorize(Policy = "User")] // TODO: Change to Admin or elevated User
        public async Task<IActionResult> GetAllUsers()
        {
            return await _helpers.HandleRequestAsync(
                userService.GetAllUsersAsync,
                "Finding users",
                "Users found successfully.",
                200,
                HttpContext.Request.Path
            );
        }

        [HttpGet("{username}")]
        [ActionName("get")]
        [EndpointDescription("Fetches a specific user")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> GetUser(string username)
        {
            return await _helpers.HandleRequestAsync(
                () => userService.GetUser(username),
                "Finding user",
                "User found successfully.",
                200,
                HttpContext.Request.Path
            );
        }

        [HttpPatch("{username}")]
        [ActionName("update")]
        [EndpointDescription("Updates an existing user")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> UpdateUser(string username, UpdateUserRequest updatedUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return await _helpers.HandleRequestAsync(
                () => userService.UpdateUser(username, updatedUser),
                "Updating user",
                "User updated successfully.",
                200,
                HttpContext.Request.Path
            );
        }

        [HttpDelete("{username}")]
        [ActionName("delete")]
        [EndpointDescription("Soft deletes a user")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            return await _helpers.HandleRequestAsync(
                () => userService.DeleteUser(username),
                "Deleting user",
                "User deleted successfully.",
                200,
                HttpContext.Request.Path
            );
        }
    }
}
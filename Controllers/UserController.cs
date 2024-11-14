using Microsoft.AspNetCore.Mvc;
using SeminarIntegration.DTOs;
using SeminarIntegration.Interfaces;
using SeminarIntegration.Models;
using System.ComponentModel.DataAnnotations;

namespace SeminarIntegration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController(IUserService userService) : Controller
    {
        private readonly IUserService _userService = userService;

        [HttpPost]
        [ActionName("create")]
        [EndpointDescription("Creates a new user")]
        public async Task<IActionResult> CreateUser(NewUserRequest newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return await HandleRequestAsync(
                () => _userService.CreateUser(newUser),
                "Creating user",
                "User created successfully.",
                201
            );
        }

        [HttpGet]
        [ActionName("all")]
        [EndpointDescription("Fetches all users")]
        public async Task<IActionResult> GetUsers()
        {
            return await HandleRequestAsync(
                () => _userService.GetUsersAsync(),
                "Finding users",
                "Users found successfully."
            );
        }

        [HttpGet("{username}")]
        [ActionName("get")]
        [EndpointDescription("Fetches a specific user")]
        public async Task<IActionResult> GetUser(string username)
        {
            return await HandleRequestAsync(
                () => _userService.GetUser(username),
                "Finding user",
                "User found successfully."
            );
        }

        [HttpPatch("{username}")]
        [ActionName("update")]
        [EndpointDescription("Updates an existing user")]
        public async Task<IActionResult> UpdateUser(string username, UpdateUserRequest updatedUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return await HandleRequestAsync(
                () => _userService.UpdateUser(username, updatedUser),
                "Updating user",
                "User updated successfully."
            );
        }

        [HttpDelete("{username}")]
        [ActionName("delete")]
        [EndpointDescription("Soft deletes a user")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            return await HandleRequestAsync(
                () => _userService.DeleteUser(username),
                "Deleting user",
                "User deleted successfully."
            );
        }


        // Helper methods
        private async Task<IActionResult> HandleRequestAsync<T>(Func<Task<T>> action, string successTitle, string successMessage, int successStatusCode = 200)
        {
            AppResponse<T>.BaseResponse response;
            var fullPath = $"{HttpContext.Request.Path}";
            try
            {
                var result = await action();
                response = new AppResponse<T>.SuccessResponse
                {
                    Title = successTitle,
                    Path = fullPath,
                    StatusCode = successStatusCode,
                    Message = successMessage,
                    Data = result
                };
                return StatusCode(successStatusCode, response);
            }
            catch (ValidationException ex)
            {
                response = new AppResponse<T>.ErrorResponse
                {
                    Title = $"Error {successTitle.ToLower()}",
                    Path = fullPath,
                    StatusCode = 400,
                    Message = ex.Message
                };
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                response = new AppResponse<T>.ErrorResponse
                {
                    Title = $"Error {successTitle.ToLower()}",
                    Path = fullPath,
                    StatusCode = 500,
                    Message = ex.Message
                };
                return StatusCode(response.StatusCode, response);
            }
        }
    }
}
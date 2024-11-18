using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using SeminarIntegration.Models;

namespace SeminarIntegration.Controllers;

public class ControllerHelpers : ControllerBase
{
    // Helper methods
    public async Task<IActionResult> HandleRequestAsync<T>(Func<Task<T>> action, string successTitle,
        string successMessage, int successStatusCode = 200, PathString pathString = default)
    {
        AppResponse<T>.BaseResponse response;
        var fullPath = $"{pathString}";
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
        catch (InvalidCredentialException ex)
        {
            response = new AppResponse<T>.ErrorResponse
            {
                Title = $"Error {successTitle.ToLower()}",
                Path = fullPath,
                StatusCode = 401,
                Message = ex.Message
            };
            return StatusCode(response.StatusCode, response);
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
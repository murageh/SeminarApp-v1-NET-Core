using SeminarIntegration.DTOs;
using SeminarIntegration.DTOs.Authentication;
using SeminarIntegration.Models;

namespace SeminarIntegration.Interfaces;

public interface IAuthService
{
    string GenerateToken(User user);
    Task<AppResponse<AuthDTOs.TokenResponse>.BaseResponse> AuthenticateUserAndReturnToken(string username, string password);
    Task<AppResponse<NormalUserResponse>.BaseResponse> RegisterUser(NewUserRequest newUserRequest);
    Task<AppResponse<NormalUserResponse>.BaseResponse> GetUserProfile(string username);
}
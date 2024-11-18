using SeminarIntegration.DTOs;
using SeminarIntegration.DTOs.Authentication;

namespace SeminarIntegration.Interfaces;

public interface IAuthService
{
    string GenerateToken(User user);
    Task<AuthDTOs.TokenResponse> AuthenticateUserAndReturnToken(string username, string password);
    public Task<NormalUserResponse> RegisterUser(NewUserRequest newUserRequest);
    public Task<NormalUserResponse> GetUserProfile(string username);
}
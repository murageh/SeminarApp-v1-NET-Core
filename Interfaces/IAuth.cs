using SeminarIntegration.DTOs;
using System.Security.Claims;
using SeminarIntegration.DTOs.Auth;

namespace SeminarIntegration.Interfaces
{
    public interface IAuthService
    {
        string GenerateToken(User user);
        Task<AuthDTOs.TokenResponse> AuthenticateUserAndReturnToken(string username, string password);
        public Task<NormalUserResponse> RegisterUser(NewUserRequest newUserRequest);
    }
}

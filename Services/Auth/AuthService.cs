using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SeminarIntegration.Data;
using SeminarIntegration.DTOs;
using SeminarIntegration.DTOs.Authentication;
using SeminarIntegration.Interfaces;
using SeminarIntegration.Models;
using SeminarIntegration.Utils;

namespace SeminarIntegration.Services.Auth
{
    public class AuthService(
        UserDbContext context,
        ILogger<UserService> logger,
        IMapper mapper,
        IConfiguration config,
        IOptions<AuthSettings> _authSettings,
        IUserService userService)
        : IAuthService
    {
        private readonly AuthSettings authSettings = _authSettings.Value;
        private const int ExpiresInSeconds = 6 * 60 * 60; // 6 hours

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("123456789ABCDEF101112131415161718191A"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = config["AuthSettings:Issuer"],
                Audience = config["AuthSettings:Audience"],
                Subject = GenerateClaims(user),
                Expires = DateTime.UtcNow.AddSeconds(ExpiresInSeconds), // Token expires in 6 hours
                SigningCredentials = credentials,
                IssuedAt = DateTime.UtcNow,
                IncludeKeyIdInHeader = true,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static ClaimsIdentity GenerateClaims(User user)
        {
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            claims.AddClaim(new Claim(ClaimTypes.Actor, user.Uuid.ToString()));

            foreach (var role in user.Role.Split(","))
            {
                claims.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        public async Task<AuthDTOs.TokenResponse> AuthenticateUserAndReturnToken(string username, string password)
        {
            var user = await userService.GetUser(username, password);
            if (user == null)
            {
                throw new InvalidCredentialException("Invalid username or password");
            }

            var token = GenerateToken(user);
            return new AuthDTOs.TokenResponse()
            {
                Token = token,
                IssuedAt = DateTime.UtcNow,
                ExpiresIn = ExpiresInSeconds,
                RefreshToken = ""
            };
        }

        public async Task<NormalUserResponse> RegisterUser(NewUserRequest newUserRequest)
        {
            return await userService.CreateUser(newUserRequest);
        }

        public async Task<NormalUserResponse> GetUserProfile(string username)
        {
            return await userService.GetUser(username);
        }
    }
}
using SeminarIntegration.DTOs;

namespace SeminarIntegration.Interfaces;

public interface IUserService
{
    Task<IEnumerable<NormalUserResponse>> GetUsersAsync();
    Task<IEnumerable<ElevatedNormalUserResponse>> GetAllUsersAsync();
    Task<NormalUserResponse> GetUser(string username);
    Task<User?> GetUser(string username, string password);
    Task<NormalUserResponse> CreateUser(NewUserRequest newUserRequest);
    Task<NormalUserResponse> UpdateUser(string username, UpdateUserRequest updatedUser);
    Task<NormalUserResponse> DeleteUser(string username);
}
using SeminarIntegration.DTOs;

namespace SeminarIntegration.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<NormalUserResponse>> GetUsersAsync();
        Task<NormalUserResponse> GetUser(string username);
        Task<NormalUserResponse> CreateUser(NewUserRequest newUserRequest);
        Task<NormalUserResponse> UpdateUser(string username, UpdateUserRequest updatedUser);
        Task<NormalUserResponse> DeleteUser(string username);
    }
}

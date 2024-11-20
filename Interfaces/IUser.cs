using SeminarIntegration.DTOs;
using SeminarIntegration.Models;

namespace SeminarIntegration.Interfaces;

public interface IUserService
{
    Task<AppResponse<List<NormalUserResponse>>.BaseResponse> GetUsersAsync();
    Task<AppResponse<List<ElevatedNormalUserResponse>>.BaseResponse> GetAllUsersAsync();
    Task<AppResponse<NormalUserResponse>.BaseResponse> GetUser(string username);
    Task<User?> GetUser(string username, string password);
    Task<AppResponse<NormalUserResponse>.BaseResponse> CreateUser(NewUserRequest newUserRequest);
    Task<AppResponse<NormalUserResponse>.BaseResponse> UpdateUser(string username, UpdateUserRequest updatedUser);
    Task<AppResponse<NormalUserResponse>.BaseResponse> DeleteUser(string username);
}
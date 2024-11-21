using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SeminarIntegration.DTOs;
using SeminarIntegration.Interfaces;
using SeminarIntegration.Models;
using SeminarIntegration.Utils;

namespace SeminarIntegration.Services;

public class UserService(HttpClient httpClient, IConfiguration config, Credentials credentials, IMapper mapper)
    : IUserService
{
    private readonly UrlHelper _urlHelper = new(config);

    public async Task<AppResponse<List<NormalUserResponse>>.BaseResponse> GetUsersAsync()
    {
        var url = $"{Connection.BaseUri}{Connection.AppUserListPath}";
        // url += FilterHelper.GenerateFilter("IsDeleted", "false", true); // TODO: Come back to this

        var responseWrapper = await HttpHelper.SendGetRequest<BcJsonResponse>(url);

        if (!responseWrapper.IsSuccess)
            return AppResponse<List<NormalUserResponse>>.Failure(responseWrapper.ErrorMessage,
                (int)responseWrapper.StatusCode,
                "Get Users Failed", url);

        // extract users
        JToken? tk = responseWrapper.Data?.value;
        var userList = tk?.ToObject<List<User>>();
        var users = new List<NormalUserResponse>();
        if (userList == null)
            return AppResponse<List<NormalUserResponse>>.Success(users, "Operation successful.", (int)HttpStatusCode.OK,
                "Get Seminars Success", url);

        users.AddRange(userList.Select(mapper.Map<NormalUserResponse>));

        return AppResponse<List<NormalUserResponse>>.Success(users, "Operation successful.", (int)HttpStatusCode.OK,
            "Get Users Success", url);
    }

    public async Task<AppResponse<List<ElevatedNormalUserResponse>>.BaseResponse> GetAllUsersAsync()
    {
        var url = $"{Connection.BaseUri}{Connection.AppUserListPath}";
        var responseWrapper = await HttpHelper.SendGetRequest<BcJsonResponse>(url);
        if (!responseWrapper.IsSuccess)
            return AppResponse<List<ElevatedNormalUserResponse>>.Failure(responseWrapper.ErrorMessage,
                (int)responseWrapper.StatusCode,
                "Get Users Failed", url);
        // extract users
        JToken? tk = responseWrapper.Data?.value;
        var userList = tk?.ToObject<List<User>>();
        var users = new List<ElevatedNormalUserResponse>();
        if (userList == null)
            return AppResponse<List<ElevatedNormalUserResponse>>.Success(users, "Operation successful.",
                (int)HttpStatusCode.OK,
                "Get Users Success", url);
        users.AddRange(userList.Select(mapper.Map<ElevatedNormalUserResponse>));
        return AppResponse<List<ElevatedNormalUserResponse>>.Success(users, "Operation successful.",
            (int)HttpStatusCode.OK,
            "Get Users Success", url);
    }

    public async Task<AppResponse<NormalUserResponse>.BaseResponse> GetUser(string username)
    {
        var url = $"{Connection.BaseUri}{Connection.AppUserListPath}";
        url += FilterHelper.GenerateFilter("Username", username, true);
        var responseWrapper = await HttpHelper.SendGetRequest<BcJsonResponse>(url);
        if (!responseWrapper.IsSuccess)
            return AppResponse<NormalUserResponse>.Failure(responseWrapper.ErrorMessage,
                (int)responseWrapper.StatusCode,
                "Get User Failed", url);
        // extract users
        // JToken? tk = responseWrapper.Data?.value;
        // var userList = tk?.ToObject<List<User>>();
        var result = responseWrapper.Data?.value.ToString();
        List<User> userList = [];
        foreach (var u in responseWrapper.Data?.value)
        {
           userList.Add(JsonConvert.DeserializeObject<User>(u.ToString())); 
        }
        if (userList.Count == 0)
            return AppResponse<NormalUserResponse>.Failure("User not found", (int)HttpStatusCode.NotFound,
                "Get User Failed", url);
        var user = userList.FirstOrDefault();
        if (user == null)
            return AppResponse<NormalUserResponse>.Failure("User not found", (int)HttpStatusCode.NotFound,
                "Get User Failed", url);
        return AppResponse<NormalUserResponse>.Success(mapper.Map<NormalUserResponse>(user), "Operation successful.",
            (int)HttpStatusCode.OK, "Get User Success", url);
    }

    public async Task<AppResponse<NormalUserResponse>.BaseResponse> CreateUser(NewUserRequest newUserRequest)
    {
        if (
            string.IsNullOrWhiteSpace(newUserRequest.Email) ||
            string.IsNullOrWhiteSpace(newUserRequest.Password) ||
            string.IsNullOrWhiteSpace(newUserRequest.Username) ||
            string.IsNullOrWhiteSpace(newUserRequest.Name)
        )
            throw new ValidationException(
                "Email, password, username, first name, and last name are required fields.");

        var hashedPwd =
            BCrypt.Net.BCrypt.EnhancedHashPassword(newUserRequest.Password, HashType.SHA384);

        var functionName = "CreateUser";
        var url = _urlHelper.GenerateUnboundUrl(functionName);
        var responseWrapper = await HttpHelper.SendPostRequest<BcJsonResponse>(url,
            new
            {
                username = newUserRequest.Username,
                password = hashedPwd,
                email = newUserRequest.Email,
                name = newUserRequest.Name
            }
        );

        if (!responseWrapper.IsSuccess)
            return AppResponse<NormalUserResponse>.Failure(responseWrapper.ErrorMessage,
                (int)responseWrapper.StatusCode,
                "Create user Failed", url);

        // extract user
        UserResponseValue? semResponse =
            JsonConvert.DeserializeObject<UserResponseValue>(responseWrapper.Data?.value);
        var user = ExtractUserFromResponseWrapper(semResponse);
        if (user == null)
            return AppResponse<NormalUserResponse>.Failure("User not found", (int)HttpStatusCode.NotFound,
                "Create user Failed", url);

        var mappedUser = mapper.Map<NormalUserResponse>(user);
        return AppResponse<NormalUserResponse>.Success(mappedUser, "Operation successful.",
            (int)HttpStatusCode.OK, "Create user Success", url);
    }

    public async Task<AppResponse<NormalUserResponse>.BaseResponse> UpdateUser(string username, UpdateUserRequest updatedUser)
    {
        var url = $"{Connection.BaseUri}{Connection.AppUserListPath}";
        url += FilterHelper.GenerateFilter("Username", username, true);
        var responseWrapper = await HttpHelper.SendPostRequest<BcJsonResponse>(url,
            new
            {
                username,
                name = updatedUser.Name,
            }
        );
        if (!responseWrapper.IsSuccess)
            return AppResponse<NormalUserResponse>.Failure(responseWrapper.ErrorMessage,
                (int)responseWrapper.StatusCode,
                "Update user Failed", url);

        // extract user
        JToken? tk = responseWrapper.Data?.value;
        var user = tk?.ToObject<User>();
        if (user == null)
            return AppResponse<NormalUserResponse>.Failure("User not found", (int)HttpStatusCode.NotFound,
                "Update user Failed", url);

        var mappedUser = mapper.Map<NormalUserResponse>(user);
        return AppResponse<NormalUserResponse>.Success(mappedUser, "Operation successful.",
            (int)HttpStatusCode.OK, "Update user Success", url);
    }

    public async Task<AppResponse<NormalUserResponse>.BaseResponse> DeleteUser(string username)
    {
        var url = $"{Connection.BaseUri}{Connection.AppUserListPath}";
        url += FilterHelper.GenerateFilter("Username", username, true);
        var responseWrapper = await HttpHelper.SendPostRequest<BcJsonResponse>(url,
            new
            {
                username
            }
        );
        if (!responseWrapper.IsSuccess)
            return AppResponse<NormalUserResponse>.Failure(responseWrapper.ErrorMessage,
                (int)responseWrapper.StatusCode,
                "Delete user Failed", url);
        // extract user
        JToken? tk = responseWrapper.Data?.value;
        var user = tk?.ToObject<User>();
        if (user == null)
            return AppResponse<NormalUserResponse>.Failure("User not found", (int)HttpStatusCode.NotFound,
                "Delete user Failed", url);
        var mappedUser = mapper.Map<NormalUserResponse>(user);
        return AppResponse<NormalUserResponse>.Success(mappedUser, "Operation successful.",
            (int)HttpStatusCode.OK, "Delete user Success", url);
    }

    // For auth
    public async Task<User?> GetUser(string username, string password)
    {
        var url = $"{Connection.BaseUri}AuthList";
        url += FilterHelper.GenerateFilter("Username", username, true);
        var responseWrapper = await HttpHelper.SendGetRequest<BcJsonResponse>(url);
        
        if (!responseWrapper.IsSuccess)
            return null;
        
        // extract users
        JToken? tk = responseWrapper.Data?.value;
        var userList = tk?.ToObject<List<User>>();

        var user = userList?.FirstOrDefault();
        if (user == null)
            return null;
        
        if (!BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password))
            return null;
        return user;
    }

    private User? ExtractUserFromResponseWrapper(UserResponseValue? res)
    {
        if (res == null) return default;
        JToken? tk = res.data;
        return tk?.ToObject<User>() ?? default;
    }
}

public class UserResponseValue
{
    public int status { get; set; }
    public string message { get; set; }
    public dynamic? data { get; set; }
}
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Azure.Core;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SeminarIntegration.Data;
using SeminarIntegration.DTOs;
using SeminarIntegration.Interfaces;

namespace SeminarIntegration.Services
{
    public class UserService(UserDbContext context, ILogger<UserService> logger, IMapper mapper)
        : IUserService
    {
        private async Task<IEnumerable<User>> _fetchUsers(bool includeDeleted)
        {
            var users = await context.Users
                .Where(u => includeDeleted || !u.IsDeleted)
                .ToListAsync();
            if (users == null)
            {
                throw new ValidationException("No users found");
            }

            return users;
        }

        public async Task<IEnumerable<NormalUserResponse>> GetUsersAsync()
        {
            var users = await _fetchUsers(false);

            var lst = users
                .Select(u => mapper.Map<NormalUserResponse>(u))
                .ToList();

            return lst;
        }

        public async Task<IEnumerable<ElevatedNormalUserResponse>> GetAllUsersAsync()
        {
            var users = await _fetchUsers(true);
            var lst = users
                .Select(u => mapper.Map<ElevatedNormalUserResponse>(u))
                .ToList();
            return lst;
        }

        public async Task<NormalUserResponse> GetUser(string username)
        {
            var user = await context.Users
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                throw new ValidationException("User not found");
            }

            return mapper.Map<NormalUserResponse>(user);
        }

        public async Task<NormalUserResponse> CreateUser(NewUserRequest newUserRequest)
        {
            User? user;
            try
            {
                if (
                    string.IsNullOrWhiteSpace(newUserRequest.Email) ||
                    string.IsNullOrWhiteSpace(newUserRequest.Password) ||
                    string.IsNullOrWhiteSpace(newUserRequest.Username) ||
                    string.IsNullOrWhiteSpace(newUserRequest.FirstName) ||
                    string.IsNullOrWhiteSpace(newUserRequest.LastName)
                )
                {
                    throw new ValidationException(
                        "Email, password, username, first name, and last name are required fields.");
                }

                string hashedPwd =
                    BCrypt.Net.BCrypt.EnhancedHashPassword(newUserRequest.Password, hashType: HashType.SHA384);

                // Check if a user with the same username or email exists and is deleted
                user = await context.Users
                    .Where(u => u.Username == newUserRequest.Username || u.Email == newUserRequest.Email)
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    if (user.IsDeleted)
                    {
                        // Reactivate the deleted user
                        user.IsDeleted = false;
                        user.DeletedAt = null;
                        user.UpdatedAt = DateTime.UtcNow;
                        user.Password = hashedPwd;
                        user.FirstName = newUserRequest.FirstName;
                        user.LastName = newUserRequest.LastName;
                        user.Title = newUserRequest.Title;
                        user.Email = newUserRequest.Email;
                        user.PreviouslyDeleted = true; // Set PreviouslyDeleted flag
                        context.Users.Update(user);
                    }
                    else
                    {
                        throw new ValidationException("A user with the same username or email already exists.");
                    }
                }
                else
                {
                    // Proceed with creating a new user
                    user = mapper.Map<NewUserRequest, User>(newUserRequest);
                    user.Password = hashedPwd;
                    user.Title ??= "";
                    user.CreatedAt = DateTime.UtcNow;
                    context.Users.Add(user);
                }

                await context.SaveChangesAsync();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating the user.");
                throw new Exception("An error occurred while creating the user.");
            }

            return mapper.Map<NormalUserResponse>(user);
        }

        public async Task<NormalUserResponse> UpdateUser(string username, UpdateUserRequest updatedUser)
        {
            User? user;

            try
            {
                user = await context.Users
                    .Where(u => !u.IsDeleted)
                    .FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    throw new ValidationException("User not found");
                }

                // Proceed
                user = mapper.Map(updatedUser, user);
                user.UpdatedAt = DateTime.UtcNow;
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating the user.");
                throw new Exception("An error occurred while updating the user.");
            }

            return mapper.Map<NormalUserResponse>(user);
        }

        public async Task<NormalUserResponse?> DeleteUser(string username)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    throw new ValidationException("User not found");
                }
                else if (user.IsDeleted)
                {
                    return default;
                }

                // Copy user record to DeletedUserHistory table
                var deletedUserHistory = DeletedUserHistory.FromUser(user);
                context.DeletedUserHistory.Add(deletedUserHistory);

                // Mark user as deleted
                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting the user.");
                throw new Exception("An error occurred while deleting the user.");
            }

            return default;
        }

        // For auth
        public async Task<User?> GetUser(string username, string password)
        {
            var user = await context.Users
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(u => u.Username == username);
            
            if (user == null) return default;

            if (BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password))
                return user;
            else
                return default;
        }
    }
}
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using SeminarIntegration.Data;
using SeminarIntegration.DTOs;
using SeminarIntegration.Interfaces;

namespace SeminarIntegration.Services
{
    public class UserService(UserDbContext context, ILogger<UserService> logger, IMapper mapper)
        : IUserService
    {
        private readonly UserDbContext _context = context;
        private readonly ILogger<UserService> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<NormalUserResponse>> GetUsersAsync()
        {
            var users = await _context.Users
                .Where(u => !u.IsDeleted)
                .ToListAsync();
            if (users == null)
            {
                throw new ValidationException("No users found");
            }

            var lst = users
                .Select(u => _mapper.Map<NormalUserResponse>(u))
                .ToList();
            return lst;
        }

        public async Task<NormalUserResponse> GetUser(string username)
        {
            var user = await _context.Users
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                throw new ValidationException("User not found");
            }

            return _mapper.Map<NormalUserResponse>(user);
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

                // Check if a user with the same username or email exists and is deleted
                user = await _context.Users
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
                        user.Password = newUserRequest.Password; // Update password if needed
                        user.FirstName = newUserRequest.FirstName;
                        user.LastName = newUserRequest.LastName;
                        user.Title = newUserRequest.Title;
                        user.Email = newUserRequest.Email;
                        user.PreviouslyDeleted = true; // Set PreviouslyDeleted flag
                        _context.Users.Update(user);
                    }
                    else
                    {
                        throw new ValidationException("A user with the same username or email already exists.");
                    }
                }
                else
                {
                    // Proceed with creating a new user
                    user = _mapper.Map<NewUserRequest, User>(newUserRequest);
                    user.Title ??= "";
                    user.CreatedAt = DateTime.UtcNow;
                    _context.Users.Add(user);
                }

                await _context.SaveChangesAsync();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the user.");
                throw new Exception("An error occurred while creating the user.");
            }

            return _mapper.Map<NormalUserResponse>(user);
        }

        public async Task<NormalUserResponse> UpdateUser(string username, UpdateUserRequest updatedUser)
        {
            User? user;

            try
            {
                user = await _context.Users
                    .Where(u => !u.IsDeleted)
                    .FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    throw new ValidationException("User not found");
                }

                // Proceed
                user = _mapper.Map(updatedUser, user);
                user.UpdatedAt = DateTime.UtcNow;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user.");
                throw new Exception("An error occurred while updating the user.");
            }

            return _mapper.Map<NormalUserResponse>(user);
        }

        public async Task<NormalUserResponse?> DeleteUser(string username)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
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
                _context.DeletedUserHistory.Add(deletedUserHistory);

                // Mark user as deleted
                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user.");
                throw new Exception("An error occurred while deleting the user.");
            }

            return default;
        }
    }
}
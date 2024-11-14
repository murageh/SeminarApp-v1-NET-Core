using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SeminarIntegration.DTOs
{
    public class User
    {
        /// <summary>
        /// The user class will be used by end-users to access the system.
        /// Authentication and authorization will be implemented using/from this class
        /// </summary>
        public User(string username,
            string password,
            string email,
            string firstName,
            string lastName,
            string? title
        )
        {
            Username = username;
            Password = password;
            Email = email;
            Title = title ?? "";
            FirstName = firstName;
            LastName = lastName;
            Role = "user";
        }

        [Key] public string Uuid { get; set; } = Guid.NewGuid().ToString("B").ToUpper();
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? DeletedAt { get; set; }
        public bool PreviouslyDeleted { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Role { get; set; }

        // Util methods
        public bool IsAdmin() => Role == "admin";
        public bool IsUser() => Role == "user";
    }

    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class NewUserRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required] [StringLength(250)] public string Password { get; set; }
        [Required] [EmailAddress] public string Email { get; set; }
        [Required] [StringLength(30)] public string FirstName { get; set; }
        [Required] [StringLength(30)] public string LastName { get; set; }

        [StringLength(10)] public string? Title { get; set; }
        // [Required]
        // [AllowedValues(new string[] { "admin", "user", "guest" })]
        // public string? Role { get; set; }
    }

    // user Responses
    public class NormalUserResponse
    {
        public string Uuid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }

    // Response for User Creation
    public class NewUserResponse : NormalUserResponse
    {
    }

    public class UpdateUserRequest
    {
        [StringLength(10)] public string? Title { get; set; }
        [StringLength(30)] public string? FirstName { get; set; }
        [StringLength(30)] public string? LastName { get; set; }
    }

    public class UpdateEmailRequest
    {
        [Required] [EmailAddress] public string Email { get; set; }
    }

    public class UpdateUsernameRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }
    }

    public class UpdatePasswordRequest
    {
        [Required] [StringLength(250)] public string Password { get; set; }
    }

    public class AllowedValuesAttribute(string[] allowedValues) : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || !allowedValues.Contains(value.ToString()))
            {
                return new ValidationResult(
                    $"The field {validationContext.DisplayName} must be one of the following values: {string.Join(", ", allowedValues)}.");
            }

            return ValidationResult.Success;
        }
    }


    public class DeletedUserHistory
    {
        [Key] public string Id { get; set; } = Guid.NewGuid().ToString("B").ToUpper();
        public string Uuid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DeletedAt { get; set; }


        // factory method extract details from user. Accepts a user object
        public static DeletedUserHistory FromUser(User user)
        {
            return new DeletedUserHistory
            {
                Id = Guid.NewGuid().ToString("B").ToUpper(),
                Uuid = user.Uuid,
                Username = user.Username,
                Email = user.Email,
                Title = user.Title,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DeletedAt = DateTime.UtcNow
            };
        }
    }
}
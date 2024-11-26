using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SeminarIntegration.DTOs;

public class User
{
    /// <summary>
    ///     The user class will be used by end-users to access the system.
    ///     Authentication and authorization will be implemented using/from this class
    ///     UPDATE(20/11/2024): This User model will now be from Microsoft Dynamics BC
    /// </summary>
    public string Uuid { get; set; }
    [JsonProperty("Name")]
    public dynamic Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? DeletedAt { get; set; }
    public bool PreviouslyDeleted { get; set; }

    public string Role { get; set; }

    public string Customer_No { get; set; }
    public string Customer_Name { get; set; }
    public string Contact_No { get; set; }
    public string Contact_Name { get; set; }

    // Util methods
    public bool IsAdmin()
    {
        return Role == "admin";
    }

    public bool IsUser()
    {
        return Role == "user";
    }
}

public class LoginRequest
{
    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }
}

public class NewUserRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; }

    [Required] [StringLength(250)] public string Password { get; set; }
    [Required] [EmailAddress] public string Email { get; set; }
    [Required] [StringLength(30)] public string Name { get; set; }

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
    public string Name { get; set; }
    public string Role { get; set; }
    public string Customer_No { get; set; }
    public string Customer_Name { get; set; }
    public string Contact_No { get; set; }
    public string Contact_Name { get; set; }
}

public class ElevatedNormalUserResponse : NormalUserResponse
{
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? DeletedAt { get; set; }
    public bool PreviouslyDeleted { get; set; }
}

// Response for User Creation
public class NewUserResponse : NormalUserResponse
{
}

public class UpdateUserRequest
{
    [StringLength(30)] public string? Name { get; set; }
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
            return new ValidationResult(
                $"The field {validationContext.DisplayName} must be one of the following values: {string.Join(", ", allowedValues)}.");

        return ValidationResult.Success;
    }
}

public class DeletedUserHistory
{
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString("B").ToUpper();
    public string Uuid { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string DeletedAt { get; set; }


    // factory method extract details from user. Accepts a user object
    public static DeletedUserHistory FromUser(User user)
    {
        return new DeletedUserHistory
        {
            Id = Guid.NewGuid().ToString("B").ToUpper(),
            Uuid = user.Uuid,
            Username = user.Username,
            Email = user.Email,
            Name = user.Name, 
            DeletedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
        };
    }
}
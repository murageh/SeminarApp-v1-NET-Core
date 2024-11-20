using System.ComponentModel.DataAnnotations;

namespace SeminarIntegration.DTOs;

public class User
{
    /// <summary>
    ///     The user class will be used by end-users to access the system.
    ///     Authentication and authorization will be implemented using/from this class
    ///     UPDATE(20/11/2024): This User model will now be from Microsoft Dynamics BC
    /// </summary>
    public User(string username,
        string password,
        string email,
        string Name
    )
    {
        Username = username;
        Password = password;
        Email = email;
        Name = Name;
        Role = "user";
    }

    public string Uuid { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime? DeletedAt { get; set; }
    public bool PreviouslyDeleted { get; set; }

    public string Role { get; set; }

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
}

public class ElevatedNormalUserResponse : NormalUserResponse
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime? DeletedAt { get; set; }
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
            Name = user.Name, 
            DeletedAt = DateTime.UtcNow
        };
    }
}
namespace SeminarIntegration.DTOs.Authentication;

public class AuthDTOs
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public DateTime IssuedAt { get; set; }
        public int ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
    }
}
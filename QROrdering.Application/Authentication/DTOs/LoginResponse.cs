namespace QROrdering.Application.Authentication.DTOs
{
    public class LoginResponse
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string AccessToken { get; set; } = null!;
    }
}

namespace QROrdering.Application.Authentication.DTOs
{
    public class RegisterResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}

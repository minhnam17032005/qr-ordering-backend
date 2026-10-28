public class UserLookupResponse
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    // Trạng thái tài khoản toàn hệ thống
    public bool IsActive { get; set; }
}
namespace QROrdering.Application.Platform.Authentication.DTOs.Requests
{
    public class PlatformAdminChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = null!;

        public string NewPassword { get; set; } = null!;

        public string ConfirmPassword { get; set; } = null!;

        public string VerificationToken { get; set; } = null!;
    }
}

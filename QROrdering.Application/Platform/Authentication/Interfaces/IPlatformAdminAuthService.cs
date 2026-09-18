
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Platform.Authentication.DTOs.Requests;
using QROrdering.Application.Platform.Authentication.DTOs.Responses;

namespace QROrdering.Application.Platform.Authentication.Interfaces
{
    public interface IPlatformAdminAuthService
    {
        Task<(PlatformAdminLoginResponse response, string platformRefreshToken)> LoginAsync(
                PlatformAdminLoginRequest request);

        Task<(PlatformAdminRefreshResponse response, string refreshToken)> RefreshTokenAsync(string refreshToken);

        Task<PlatformAdminProfileResponse> GetProfileAsync();

        Task LogoutAsync();

        Task LogoutAllSessionsAsync();

        Task<PagedResponse<PlatformAdminSessionResponse>>GetSessionsAsync(PagedRequest request);

        Task DeleteSessionAsync(Guid sessionId);

        Task SendChangePasswordOtpAsync();

        Task<PlatformAdminVerifyOtpResponse> VerifyChangePasswordOtpAsync(
            PlatformAdminVerifyChangePasswordOtpRequest request);

        Task ChangePasswordAsync(
            PlatformAdminChangePasswordRequest request);

        Task SendForgotPasswordOtpAsync(
            PlatformAdminForgotPasswordOtpRequest request);

        Task<PlatformAdminVerifyOtpResponse> VerifyForgotPasswordOtpAsync(
            PlatformAdminVerifyForgotPasswordOtpRequest request);

        Task ForgotPasswordAsync(
            PlatformAdminForgotPasswordRequest request);
    }
}

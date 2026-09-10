using QROrdering.Application.Authentication.DTOs;
using QROrdering.Application.Common.Pagination;

namespace QROrdering.Application.Authentication.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);

        Task<(LoginResponse response, string refreshToken)> LoginAsync(
        LoginRequest request);

        Task<(RefreshResponse response, string refreshToken)> RefreshTokenAsync(
        string refreshToken);

        Task<UserProfileResponse> GetProfileAsync();

        Task LogoutAsync();

        Task LogoutAllSessionsAsync();

        Task LogoutOtherSessionsAsync();

        Task<PagedResponse<UserSessionResponse>> GetSessionsAsync(
        PagedRequest request);

        /// Revoke a specific session of the current user.
        Task DeleteSessionAsync(Guid sessionId);

    }
}

using QROrdering.Application.Authentication.DTOs;

namespace QROrdering.Application.Authentication.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);

        Task<(LoginResponse response, string refreshToken)> LoginAsync(
        LoginRequest request);

        Task<(RefreshResponse response, string refreshToken)> RefreshTokenAsync(
        string refreshToken);
    }
}

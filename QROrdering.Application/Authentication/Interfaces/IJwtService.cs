using QROrdering.Domain.Entities.Identity;

namespace QROrdering.Application.Authentication.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(
            User user,
            Guid sessionId);

        string GenerateRefreshToken();

        DateTime GetRefreshTokenExpiration();
    }
}

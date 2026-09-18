using QROrdering.Domain.Entities.Identity;
using QROrdering.Domain.Entities.Platform;

namespace QROrdering.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(
            User user,
            Guid sessionId);

        string GeneratePlatformAdminAccessToken(
            PlatformAdmin admin,
            Guid sessionId);
        string GenerateRefreshToken();

        DateTime GetRefreshTokenExpiration();
    }
}

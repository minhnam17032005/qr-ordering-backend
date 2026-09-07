using System.Security.Claims;

namespace QROrdering.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // Lấy UserId từ JWT
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            return Guid.TryParse(
                user.FindFirst("userId")?.Value,
                out var userId)
                ? userId
                : Guid.Empty;
        }

        // Lấy username từ JWT
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst("username")?.Value
                ?? string.Empty;
        }

        // Lấy SessionId từ JWT
        public static Guid GetSessionId(this ClaimsPrincipal user)
        {
            return Guid.TryParse(
                user.FindFirst("sid")?.Value,
                out var sessionId)
                ? sessionId
                : Guid.Empty;
        }

        // Lấy JWT ID
        public static string GetJti(this ClaimsPrincipal user)
        {
            return user.FindFirst("jti")?.Value
                ?? string.Empty;
        }

        // Lấy thời gian phát hành token
        public static long GetIssuedAt(this ClaimsPrincipal user)
        {
            return long.TryParse(
                user.FindFirst("iat")?.Value,
                out var iat)
                ? iat
                : 0;
        }

        // Lấy thời gian hết hạn token
        public static string GetExpiredAt(this ClaimsPrincipal user)
        {
            return user.FindFirst("exp")?.Value
                ?? string.Empty;
        }

        // Lấy loại user
        public static string GetUserType(this ClaimsPrincipal user)
        {
            return user.FindFirst("user_type")?.Value
                ?? string.Empty;
        }

        // Lấy danh sách role từ JWT
        public static List<string> GetRoles(this ClaimsPrincipal user)
        {
            return user.FindAll(ClaimTypes.Role)
                .Select(x => x.Value)
                .ToList();
        }

        // Kiểm tra user đã xác thực
        public static bool IsAuthenticated(this ClaimsPrincipal user)
        {
            return user.Identity?.IsAuthenticated ?? false;
        }
    }
}
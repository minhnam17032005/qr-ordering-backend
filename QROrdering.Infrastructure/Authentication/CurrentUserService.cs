using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using QROrdering.Application.Authentication.Interfaces;

namespace QROrdering.Infrastructure.Authentication
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Lấy ClaimsPrincipal của request hiện tại
        private ClaimsPrincipal? User =>
            _httpContextAccessor.HttpContext?.User;

        // Lấy UserId từ JWT
        public Guid UserId =>
            Guid.TryParse(
                User?.FindFirst("userId")?.Value,
                out var userId)
                ? userId
                : Guid.Empty;

        // Lấy username từ JWT
        public string Username =>
            User?.FindFirst("username")?.Value
            ?? string.Empty;

        // Lấy SessionId từ JWT
        public Guid SessionId =>
            Guid.TryParse(
                User?.FindFirst("sid")?.Value,
                out var sessionId)
                ? sessionId
                : Guid.Empty;

        // Lấy JWT ID
        public string Jti =>
            User?.FindFirst("jti")?.Value
            ?? string.Empty;

        // Lấy thời gian phát hành token
        public long IssuedAt =>
            long.TryParse(
                User?.FindFirst("iat")?.Value,
                out var iat)
                ? iat
                : 0;

        // Lấy thời gian hết hạn token
        public string ExpiredAtString =>
            User?.FindFirst("exp")?.Value
            ?? string.Empty;

        // Lấy danh sách role từ JWT
        public List<string> Roles =>
            User?.FindAll(ClaimTypes.Role)
                .Select(x => x.Value)
                .ToList()
            ?? new List<string>();

        // Kiểm tra user đã xác thực
        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated ?? false;
    }
}
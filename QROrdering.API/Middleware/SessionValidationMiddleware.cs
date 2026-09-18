using System.Text.Json;

using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.Platform.Authentication.Interfaces;

namespace QROrdering.API.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ISessionCacheService sessionCacheService,
            IPlatformAdminSessionCacheService
                platformAdminSessionCacheService)
        {
            // Chỉ kiểm tra đối với request đã được
            // JWT Authentication xác thực
            if (context.User.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            // =========================
            // CHECK USER TYPE
            // =========================

            var userType =
                context.User.FindFirst("user_type")?.Value;

            if (userType == "restaurant_user")
            {
                await ValidateUserSession(
                    context,
                    sessionCacheService);

                return;
            }

            if (userType == "platform_admin")
            {
                await ValidatePlatformAdminSession(
                    context,
                    platformAdminSessionCacheService);

                return;
            }

            await WriteUnauthorized(
                context,
                "INVALID_USER_TYPE",
                "Loại tài khoản không hợp lệ.");
        }

        // =========================
        // VALIDATE USER SESSION
        // =========================

        private async Task ValidateUserSession(
            HttpContext context,
            ISessionCacheService sessionCacheService)
        {
            // =========================
            // GET USER ID
            // =========================

            var userIdClaim =
                context.User.FindFirst("userId")?.Value;

            if (!Guid.TryParse(
                    userIdClaim,
                    out var userId))
            {
                await WriteUnauthorized(
                    context,
                    "INVALID_USER",
                    "Thông tin người dùng không hợp lệ.");

                return;
            }

            // =========================
            // GET SESSION ID
            // =========================

            var sessionIdClaim =
                context.User.FindFirst("sid")?.Value;

            if (!Guid.TryParse(
                    sessionIdClaim,
                    out var sessionId))
            {
                await WriteUnauthorized(
                    context,
                    "SESSION_INVALID",
                    "Phiên đăng nhập không hợp lệ.");

                return;
            }

            // =========================
            // GET SESSION
            // L1 Memory -> L2 Redis -> DB
            // =========================

            var session =
                await sessionCacheService.GetAsync(sessionId);

            if (session == null)
            {
                await WriteUnauthorized(
                    context,
                    "SESSION_NOT_FOUND",
                    "Phiên đăng nhập không tồn tại.");

                return;
            }

            // =========================
            // CHECK SESSION USER
            // =========================

            if (session.UserId != userId)
            {
                await WriteUnauthorized(
                    context,
                    "SESSION_INVALID",
                    "Phiên đăng nhập không hợp lệ.");

                return;
            }

            // =========================
            // CHECK SESSION REVOKED
            // =========================

            if (session.IsRevoked)
            {
                await WriteUnauthorized(
                    context,
                    "SESSION_REVOKED",
                    "Phiên đăng nhập đã bị thu hồi.");

                return;
            }

            // =========================
            // CHECK USER ACTIVE
            // =========================

            if (!session.IsUserActive)
            {
                await WriteUnauthorized(
                    context,
                    "USER_DISABLED",
                    "Tài khoản đã bị khóa.");

                return;
            }

            // =========================
            // CHECK SESSION EXPIRATION
            // =========================

            if (session.ExpiredAt <= DateTime.UtcNow)
            {
                await WriteUnauthorized(
                    context,
                    "SESSION_EXPIRED",
                    "Phiên đăng nhập đã hết hạn.");

                return;
            }

            await _next(context);
        }

        // =========================
        // VALIDATE PLATFORM ADMIN SESSION
        // =========================

        private async Task ValidatePlatformAdminSession(
            HttpContext context,
            IPlatformAdminSessionCacheService
                platformAdminSessionCacheService)
        {
            // =========================
            // GET PLATFORM ADMIN ID
            // =========================

            var userIdClaim =
                context.User.FindFirst("userId")?.Value;

            if (!Guid.TryParse(
                    userIdClaim,
                    out var platformAdminId))
            {
                await WriteUnauthorized(
                    context,
                    "INVALID_USER",
                    "Thông tin Platform Admin không hợp lệ.");

                return;
            }

            // =========================
            // GET SESSION ID
            // =========================

            var sessionIdClaim =
                context.User.FindFirst("sid")?.Value;

            if (!Guid.TryParse(
                    sessionIdClaim,
                    out var sessionId))
            {
                await WriteUnauthorized(
                    context,
                    "SESSION_INVALID",
                    "Phiên đăng nhập không hợp lệ.");

                return;
            }

            // =========================
            // GET SESSION
            // L1 Memory -> L2 Redis -> DB
            // =========================

            var session =
                await platformAdminSessionCacheService
                    .GetAsync(sessionId);

            if (session == null)
            {
                await WriteUnauthorized(
                    context,
                    "SESSION_NOT_FOUND",
                    "Phiên đăng nhập không tồn tại.");

                return;
            }

            // =========================
            // CHECK SESSION PLATFORM ADMIN
            // =========================

            if (session.PlatformAdminId != platformAdminId)
            {
                await WriteUnauthorized(
                    context,
                    "SESSION_INVALID",
                    "Phiên đăng nhập không hợp lệ.");

                return;
            }

            // =========================
            // CHECK SESSION REVOKED
            // =========================

            if (session.IsRevoked)
            {
                await WriteUnauthorized(
                    context,
                    "SESSION_REVOKED",
                    "Phiên đăng nhập đã bị thu hồi.");

                return;
            }

            // =========================
            // CHECK PLATFORM ADMIN ACTIVE
            // =========================

            if (!session.IsPlatformAdminActive)
            {
                await WriteUnauthorized(
                    context,
                    "USER_DISABLED",
                    "Tài khoản Platform Admin đã bị khóa.");

                return;
            }

            // =========================
            // CHECK SESSION EXPIRATION
            // =========================

            if (session.ExpiresAt <= DateTime.UtcNow)
            {
                await WriteUnauthorized(
                    context,
                    "SESSION_EXPIRED",
                    "Phiên đăng nhập đã hết hạn.");

                return;
            }

            await _next(context);
        }

        // =========================
        // 401 UNAUTHORIZED RESPONSE
        // =========================

        private static async Task WriteUnauthorized(
            HttpContext context,
            string code,
            string message)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            context.Response.ContentType =
                "application/json";

            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            var response = new ErrorResponse
            {
                StatusCode =
                    StatusCodes.Status401Unauthorized,

                Code = code,

                Message = message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
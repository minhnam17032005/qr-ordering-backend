using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QROrdering.API.Extensions;
using QROrdering.Application.Authentication.DTOs.Responses;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.Exceptions;
using QROrdering.Application.Platform.Authentication.DTOs.Requests;
using QROrdering.Application.Platform.Authentication.DTOs.Responses;
using QROrdering.Application.Platform.Authentication.Interfaces;
using QROrdering.Infrastructure.Configurations;

namespace QROrdering.API.Controllers.Authentication
{
    [ApiController]
    [Route("api/platform/auth")]
    public class PlatformAuthController : ControllerBase
    {
        private readonly IPlatformAdminAuthService _platformAdminAuthService;

        // Cấu hình JWT lấy từ appsettings.json.
        // Dùng tại API để cấu hình thời gian sống của Refresh Token Cookie.
        private readonly JwtSettings _jwtSettings;
        public PlatformAuthController(
            IPlatformAdminAuthService platformAdminAuthService,
            IOptions<JwtSettings> jwtOptions)
        {
            _platformAdminAuthService = platformAdminAuthService;
            // Lấy giá trị JwtSettings đã được bind từ appsettings.json
            _jwtSettings = jwtOptions.Value;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<PlatformAdminLoginResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PlatformAdminLoginResponse>>> Login(PlatformAdminLoginRequest request)
        {
            var (response, platformRefreshToken) =
                await _platformAdminAuthService.LoginAsync(request);

            Response.Cookies.Append(
            "platformRefreshToken",
                platformRefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(
                        _jwtSettings.RefreshTokenExpirationDays)
                });

            return this.ApiOk(
                response,
                "Login successful.");
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(ApiResponse<PlatformAdminRefreshResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PlatformAdminRefreshResponse>>> Refresh()
        {
            // Lấy refresh token từ cookie
            var refreshToken =
                Request.Cookies["platformRefreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new UnauthorizedException(
                    "Refresh token không tồn tại hoặc đã bị xóa.");
            }

            // Làm mới Access Token
            var (response, newRefreshToken) =
                await _platformAdminAuthService
                    .RefreshTokenAsync(refreshToken);

            // Rotate refresh token trong cookie
            Response.Cookies.Append(
                "platformRefreshToken",
                newRefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(
                        _jwtSettings.RefreshTokenExpirationDays)
                });

            return this.ApiOk(
                response,
                "Làm mới token thành công.");
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<PlatformAdminProfileResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PlatformAdminProfileResponse>>>GetProfile()
        {
            var result =
                await _platformAdminAuthService.GetProfileAsync();

            return this.ApiOk(
                result,
                "Lấy thông tin Platform Admin thành công.");
        }

        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<object>>>Logout()
        {
            await _platformAdminAuthService.LogoutAsync();

            Response.Cookies.Delete(
                "platformRefreshToken");

            return this.ApiOk<object>(
                null,
                "Đăng xuất thành công.");
        }

        [HttpPost("logout-all-sessions")]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<object>>>LogoutAllSessions()
        {
            await _platformAdminAuthService.LogoutAllSessionsAsync();

            Response.Cookies.Delete(
                "platformRefreshToken");

            return this.ApiOk<object>(
                null,
                "Đăng xuất khỏi tất cả phiên thành công.");
        }

        [HttpGet("sessions")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<PlatformAdminSessionResponse>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PagedResponse<PlatformAdminSessionResponse>>>>GetSessions([FromQuery] PagedRequest request)
        {
            var sessions =
                await _platformAdminAuthService
                    .GetSessionsAsync(request);

            return this.ApiOk(
                sessions,
                "Lấy danh sách phiên đăng nhập thành công.");
        }

        [HttpDelete("sessions/{sessionId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>>DeleteSession(Guid sessionId)
        {
            await _platformAdminAuthService
                .DeleteSessionAsync(sessionId);

            return this.ApiOk<object>(
                null,
                "Xóa phiên đăng nhập thành công.");
        }

        //=== Change Password ===//

        [HttpPost("change-password/send-otp")]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>>SendChangePasswordOtp()
        {
            await _platformAdminAuthService
                .SendChangePasswordOtpAsync();

            return this.ApiOk<object>(
                null,
                "OTP đã được gửi.");
        }

        [HttpPost("change-password/verify-otp")]
        [ProducesResponseType(typeof(ApiResponse<PlatformAdminVerifyOtpResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PlatformAdminVerifyOtpResponse>>>VerifyChangePasswordOtp(
        [FromBody]PlatformAdminVerifyChangePasswordOtpRequest request)
        {
            var response =
                await _platformAdminAuthService
                    .VerifyChangePasswordOtpAsync(request);

            return this.ApiOk(
                response,
                "Xác thực OTP thành công.");
        }

        [HttpPost("change-password/change")]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>>ChangePassword(
        [FromBody]PlatformAdminChangePasswordRequest request)
        {
            await _platformAdminAuthService
                .ChangePasswordAsync(request);

            return this.ApiOk<object>(
                null,
                "Mật khẩu thay đổi thành công.");
        }

        //=== Forgot Password ===//
        [HttpPost("forgot-password/send-otp")]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>>SendForgotPasswordOtp(
        [FromBody]PlatformAdminForgotPasswordOtpRequest request)
        {
            await _platformAdminAuthService
                .SendForgotPasswordOtpAsync(request);

            return this.ApiOk<object>(
                null,
                "Nếu email tồn tại, OTP đã được gửi.");
        }

        [HttpPost("forgot-password/verify-otp")]
        [ProducesResponseType(typeof(ApiResponse<PlatformAdminVerifyOtpResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PlatformAdminVerifyOtpResponse>>>VerifyForgotPasswordOtp(
        [FromBody]PlatformAdminVerifyForgotPasswordOtpRequest request)
        {
            var response =
                await _platformAdminAuthService
                    .VerifyForgotPasswordOtpAsync(request);

            return this.ApiOk(
                response,
                "Xác thực OTP thành công.");
        }

        [AllowAnonymous]
        [HttpPost("forgot-password/reset")]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>>ResetForgotPassword(
        [FromBody]PlatformAdminForgotPasswordRequest request)
        {
            await _platformAdminAuthService
                .ForgotPasswordAsync(request);

            return this.ApiOk<object>(
                null,
                "Đặt lại mật khẩu thành công.");
        }


    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QROrdering.API.Extensions;
using QROrdering.Application.Authentication.DTOs;
using QROrdering.Application.Authentication.DTOs.Requests;
using QROrdering.Application.Authentication.DTOs.Responses;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.Exceptions;
using QROrdering.Infrastructure.Authentication;
using QROrdering.Infrastructure.Configurations;

namespace QROrdering.API.Controllers.Authentication
{
    [ApiController]
    [Route("api/account")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Cấu hình JWT lấy từ appsettings.json.
        // Dùng tại API để cấu hình thời gian sống của Refresh Token Cookie.
        private readonly JwtSettings _jwtSettings;

        public AuthController(
            IAuthService authService,
            IOptions<JwtSettings> jwtOptions)
        {
            _authService = authService;
            // Lấy giá trị JwtSettings đã được bind từ appsettings.json
            _jwtSettings = jwtOptions.Value;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<RegisterResponse>>> Register(
           RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            return this.ApiCreated(
                result,
                "Registration successful.");
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
            LoginRequest request)
        {
            var (response, refreshToken) =
                await _authService.LoginAsync(request);

            // Lưu refresh token vào HttpOnly cookie
            Response.Cookies.Append(
                "refreshToken",
                refreshToken,
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
        [ProducesResponseType(typeof(ApiResponse<RefreshResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<RefreshResponse>>> Refresh()
        {
            // Lấy refresh token từ cookie
            var refreshToken =
                Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new UnauthorizedException(
                    "Refresh token không tồn tại hoặc đã bị xóa.");
            }

            // Làm mới Access Token
            var (response, newRefreshToken) =
                await _authService.RefreshTokenAsync(
                    refreshToken);

            // Rotate refresh token trong cookie
            Response.Cookies.Append(
                "refreshToken",
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

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<UserProfileResponse>>> GetProfile()
        {
            var result = await _authService.GetProfileAsync();

            return this.ApiOk(
                result,
                "Lấy thông tin cá nhân thành công.");
        }

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            await _authService.LogoutAsync();

            Response.Cookies.Delete("refreshToken");

            return this.ApiOk<object>(
                null,
                "Đăng xuất thành công.");
        }

        [Authorize]
        [HttpPost("logout-all-sessions")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<object>>> LogoutAllSessions()
        {
            await _authService.LogoutAllSessionsAsync();

            Response.Cookies.Delete("refreshToken");

            return this.ApiOk<object>(
                null,
                "Đăng xuất khỏi tất cả phiên thành công.");
        }

        [Authorize]
        [HttpPost("logout-other-sessions")]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<object>>> LogoutOtherSessions()
        {
            await _authService.LogoutOtherSessionsAsync();

            return this.ApiOk<object>(
                null,
                "Đăng xuất khỏi tất cả phiên khác thành công.");
        }

        [Authorize]
        [HttpGet("sessions")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<UserSessionResponse>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PagedResponse<UserSessionResponse>>>> GetSessions(
        [FromQuery] PagedRequest request)
        {
            var sessions = await _authService.GetSessionsAsync(request);

            return this.ApiOk(
                sessions,
                "Lấy danh sách phiên đăng nhập thành công.");
        }

        [Authorize]
        [HttpDelete("sessions/{sessionId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteSession(Guid sessionId)
        {
            await _authService.DeleteSessionAsync(sessionId);

            return this.ApiOk<object>(
                null,
                "Xóa phiên đăng nhập thành công.");
        }

        //=== Change Password ===//

        [Authorize]
        [HttpPost("change-password/send-otp")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> SendChangePasswordOtp()
        {
            await _authService.SendChangePasswordOtpAsync();

            return this.ApiOk<object>(
                null,
                "Nếu email tồn tại, OTP đã được gửi.");
        }

        [Authorize]
        [HttpPost("change-password/verify-otp")]
        [ProducesResponseType(typeof(ApiResponse<VerifyOtpResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<VerifyOtpResponse>>> VerifyChangePasswordOtp(
            [FromBody] VerifyChangePasswordOtpRequest request)
        {
            var response = await _authService.VerifyChangePasswordOtpAsync(
                request);

            return this.ApiOk(
                response,
                "Xác thực OTP thành công.");
        }

        [Authorize]
        [HttpPost("change-password/change")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> ChangePassword(
            [FromBody] ChangePasswordRequest request)
        {
            await _authService.ChangePasswordAsync(request);

            return this.ApiOk<object>(
                null,
                "Mật khẩu thay đổi thành công.");
        }

        //=== Forgot Password ===//

        [AllowAnonymous]
        [HttpPost("forgot-password/send-otp")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> SendForgotPasswordOtp(
            [FromBody] ForgotPasswordOtpRequest request)
        {
            await _authService.SendForgotPasswordOtpAsync(request);

            return this.ApiOk<object>(
                null,
                "Nếu email tồn tại, OTP đã được gửi.");
        }

        [AllowAnonymous]
        [HttpPost("forgot-password/verify-otp")]
        [ProducesResponseType(typeof(ApiResponse<VerifyOtpResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<VerifyOtpResponse>>> VerifyForgotPasswordOtp(
            [FromBody] VerifyForgotPasswordOtpRequest request)
        {
            var response = await _authService.VerifyForgotPasswordOtpAsync(
                request);

            return this.ApiOk(
                response,
                "Xác thực OTP thành công.");
        }

        [AllowAnonymous]
        [HttpPost("forgot-password/reset")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> ResetForgotPassword(
            [FromBody] ForgotPasswordRequest request)
        {
            await _authService.ForgotPasswordAsync(request);

            return this.ApiOk<object>(
                null,
                "Đặt lại mật khẩu thành công.");
        }


    }
}

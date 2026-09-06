using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QROrdering.API.Common;
using QROrdering.API.Extensions;
using QROrdering.Application.Authentication.DTOs;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Application.Exceptions;
using QROrdering.Infrastructure.Authentication;
using QROrdering.Infrastructure.Configurations;

namespace QROrdering.API.Controllers.Authentication
{
    [ApiController]
    [Route("api/auth")]
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
        [ProducesResponseType(
            typeof(ApiResponse<RegisterResponse>),
            StatusCodes.Status201Created)]
        [ProducesResponseType(
            typeof(ErrorResponse),
            StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<RegisterResponse>>> Register(
           RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            return this.ApiCreated(
                result,
                "Registration successful.");
        }

        [HttpPost("login")]
        [ProducesResponseType(
            typeof(ApiResponse<LoginResponse>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ErrorResponse),
            StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(
            typeof(ApiResponse<RefreshResponse>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ErrorResponse),
            StatusCodes.Status401Unauthorized)]
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
    }
}

using Microsoft.AspNetCore.Mvc;
using QROrdering.API.Common;
using QROrdering.API.Extensions;
using QROrdering.Application.Authentication.DTOs;
using QROrdering.Application.Authentication.Interfaces;

namespace QROrdering.API.Controllers.Authentication
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
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

            Response.Cookies.Append(
                "refreshToken",
                refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)//lưu ý nên sửa khi có thời gian không nên fix cứng 
                });

            return this.ApiOk(
                response,
                "Login successful.");
        }
    }
}

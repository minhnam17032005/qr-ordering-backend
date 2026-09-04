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
    }
}

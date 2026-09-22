using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QROrdering.API.Extensions;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.Platform.Users.DTOs.Requests;
using QROrdering.Application.Platform.Users.DTOs.Responses;
using QROrdering.Application.Platform.Users.Interfaces;

namespace QROrdering.API.Controllers.Platform
{
    [ApiController]
    [Route("api/platform/users")]
    [Authorize]
    public class PlatformUsersController : ControllerBase
    {
        private readonly IPlatformUserService _userService;

        public PlatformUsersController(IPlatformUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<PlatformUserListResponse>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PagedResponse<PlatformUserListResponse>>>>GetUsers(
                [FromQuery] PagedRequest request)
        {
            var response =
                await _userService.GetUsersAsync(
                    request);

            return this.ApiOk(
                response,
                "Lấy danh sách người dùng thành công.");
        }

        [HttpGet("{userId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PlatformUserDetailResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PlatformUserDetailResponse>>>GetUser(Guid userId)
        {
            var response =
                await _userService.GetUserByIdAsync(
                    userId);

            return this.ApiOk(
                response,
                "Lấy thông tin người dùng thành công.");
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PlatformUserDetailResponse>),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<PlatformUserDetailResponse>>>CreateUser(
        CreatePlatformUserRequest request)
        {
            var response =
                await _userService.CreateUserAsync(
                    request);

            return this.ApiCreated(
                response,
                "Tạo người dùng thành công.");
        }

        [HttpPut("{userId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PlatformUserDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status409Conflict)]
        public async Task< ActionResult< ApiResponse<PlatformUserDetailResponse>>>UpdateUser(
        Guid userId,
        UpdatePlatformUserRequest request)
        {
            var response =
                await _userService.UpdateUserAsync(
                    userId,
                    request);

            return this.ApiOk(
                response,
                "Cập nhật người dùng thành công.");
        }

        [HttpPost("{userId:guid}/activate")]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>>ActivateUser(Guid userId)
        {
            await _userService.ActivateUserAsync(
                userId);

            return this.ApiOk<object>(
                null,
                "Kích hoạt người dùng thành công.");
        }

        [HttpPost("{userId:guid}/deactivate")]
        [ProducesResponseType(typeof(ApiResponse<object>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>>DeactivateUser(Guid userId)
        {
            await _userService.DeactivateUserAsync(
                userId);

            return this.ApiOk<object>(
                null,
                "Vô hiệu hóa người dùng thành công.");
        }
    }
}

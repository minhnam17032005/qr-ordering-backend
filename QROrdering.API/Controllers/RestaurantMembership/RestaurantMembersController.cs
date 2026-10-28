using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QROrdering.API.Extensions;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.Membership.DTOs.Requests;
using QROrdering.Application.Membership.DTOs.Responses;
using QROrdering.Application.Membership.Interfaces;
using QROrdering.Infrastructure.Membership.DTOs.Requests;
using QROrdering.Infrastructure.Membership.DTOs.Responses;

namespace QROrdering.API.Controllers.Restaurants
{
    [ApiController]
    [Route("api/restaurants/{restaurantId:guid}")]
    [Authorize]
    public class RestaurantMembersController : ControllerBase
    {
        private readonly IRestaurantMembershipService
            _membershipService;

        public RestaurantMembersController(
            IRestaurantMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        [HttpGet("members")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<RestaurantMemberListResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<PagedResponse<RestaurantMemberListResponse>>>> GetMembers(
                Guid restaurantId,
                [FromQuery] PagedRequest request)
        {
            var response =
                await _membershipService.GetMembersAsync(
                    restaurantId,
                    request);

            return this.ApiOk(
                response,
                "Lấy danh sách thành viên thành công.");
        }

        [HttpGet("members/{memberId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<RestaurantMemberDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<RestaurantMemberDetailResponse>>> GetMemberById(
                Guid restaurantId,
                Guid memberId)
        {
            var response =
                await _membershipService.GetMemberByIdAsync(
                    restaurantId,
                    memberId);

            return this.ApiOk(
                response,
                "Lấy thông tin thành viên thành công.");
        }



        [HttpGet("users/lookup")]
        [ProducesResponseType(typeof(ApiResponse<UserLookupResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserLookupResponse>>> LookupUser(
            Guid restaurantId,
            [FromQuery] UserLookupRequest request)
        {
            var response =
                await _membershipService.LookupUserAsync(
                    restaurantId,
                    request);

            return this.ApiOk(
                response,
                "Tra cứu người dùng thành công.");
        }

        [HttpPost("members")]
        [ProducesResponseType(typeof(ApiResponse<AddMemberResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<AddMemberResponse>>> AddMember(
            Guid restaurantId,
            [FromBody] AddMemberRequest request)
        {
            var response =
                await _membershipService.AddMemberAsync(
                    restaurantId,
                    request);

            return this.ApiCreated(
                response,
                "Thêm thành viên thành công.");
        }

        [HttpPost("members/register")]
        [ProducesResponseType(typeof(ApiResponse<RegisterMemberResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<RegisterMemberResponse>>> RegisterMember(
            Guid restaurantId,
            [FromBody] RegisterMemberRequest request)
        {
            var response =
                await _membershipService.RegisterMemberAsync(
                    restaurantId,
                    request);

            return this.ApiOk(
                response,
                "Đăng ký thành viên thành công.");
        }


        [HttpPut("members/{memberId:guid}/roles")]
        [ProducesResponseType(typeof(ApiResponse<UpdateMemberRolesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<UpdateMemberRolesResponse>>> UpdateMemberRoles(
            Guid restaurantId,
            Guid memberId,
            [FromBody] UpdateMemberRolesRequest request)
        {
            var response =
                await _membershipService.UpdateMemberRolesAsync(
                    restaurantId,
                    memberId,
                    request);

            return this.ApiOk(
                response,
                "Cập nhật vai trò thành viên thành công.");
        }

        [HttpPost("members/{memberId:guid}/activate")]
        [ProducesResponseType(typeof(ApiResponse<ActivateMemberResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<ActivateMemberResponse>>> ActivateMember(
                Guid restaurantId,
                Guid memberId)
        {
            var response = await _membershipService.ActivateMemberAsync(
                restaurantId,
                memberId);

            return this.ApiOk(
                response,
                "Kích hoạt thành viên thành công.");
        }

        [HttpPost("members/{memberId:guid}/deactivate")]
        [ProducesResponseType(typeof(ApiResponse<DeactivateMemberResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<DeactivateMemberResponse>>>
            DeactivateMember(
                Guid restaurantId,
                Guid memberId)
        {
            var response = await _membershipService.DeactivateMemberAsync(
                restaurantId,
                memberId);

            return this.ApiOk(
                response,
                "Vô hiệu hóa thành viên thành công.");
        }

        [HttpGet("roles")]
        [ProducesResponseType(typeof(ApiResponse<List<RestaurantRoleResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<List<RestaurantRoleResponse>>>> GetRoles(Guid restaurantId)
        {
            var response = await _membershipService.GetRolesAsync(restaurantId);

            return this.ApiOk(
                response,
                "Lấy danh sách Role thành công.");
        }

        [HttpGet("~/api/account/restaurants")]
        [ProducesResponseType(typeof(ApiResponse<List<AccountRestaurantResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<List<AccountRestaurantResponse>>>> GetMyRestaurants()
        {
            var response =
                await _membershipService.GetMyRestaurantsAsync();

            return this.ApiOk(
                response,
                "Lấy danh sách nhà hàng của tài khoản thành công.");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QROrdering.API.Extensions;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.Platform.Restaurants.DTOs.Requests;
using QROrdering.Application.Platform.Restaurants.DTOs.Responses;
using QROrdering.Application.Platform.Restaurants.Interfaces;

namespace QROrdering.API.Controllers.Platform
{
    [ApiController]
    [Route("api/platform/restaurants")]
    [Authorize]
    public class PlatformRestaurantsController : ControllerBase
    {
        private readonly IPlatformRestaurantService _platformRestaurantService;

        public PlatformRestaurantsController(
            IPlatformRestaurantService platformRestaurantService)
        {
            _platformRestaurantService = platformRestaurantService;
        }


        //=== Get Restaurants ===//

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<PlatformRestaurantResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResponse<PlatformRestaurantResponse>>>> GetRestaurants(
                [FromQuery] PagedRequest request)
        {
            var response =
                await _platformRestaurantService
                    .GetRestaurantsAsync(request);

            return this.ApiOk(
                response,
                "Lấy danh sách nhà hàng thành công.");
        }


        //=== Get Restaurant ===//

        [HttpGet("{restaurantId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PlatformRestaurantResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PlatformRestaurantResponse>>> GetRestaurant(Guid restaurantId)
        {
            var response =
                await _platformRestaurantService
                    .GetRestaurantByIdAsync(
                        restaurantId);

            return this.ApiOk(
                response,
                "Lấy thông tin nhà hàng thành công.");
        }


        //=== Create Restaurant ===//

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PlatformRestaurantResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PlatformRestaurantResponse>>> CreateRestaurant(
                [FromBody] CreateRestaurantRequest request)
        {
            var response =
                await _platformRestaurantService
                    .CreateRestaurantAsync(request);

            return this.ApiOk(
                response,
                "Tạo nhà hàng thành công.");
        }


        //=== Update Restaurant ===//

        [HttpPut("{restaurantId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PlatformRestaurantResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PlatformRestaurantResponse>>> UpdateRestaurant(
            Guid restaurantId, [FromBody] UpdateRestaurantRequest request)
        {
            var response =
                await _platformRestaurantService
                    .UpdateRestaurantAsync(
                        restaurantId,
                        request);

            return this.ApiOk(
                response,
                "Cập nhật nhà hàng thành công.");
        }


        //=== Activate Restaurant ===//

        [HttpPost("{restaurantId:guid}/activate")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> ActivateRestaurant(Guid restaurantId)
        {
            await _platformRestaurantService
                .ActivateRestaurantAsync(
                    restaurantId);

            return this.ApiOk<object>(
                null,
                "Kích hoạt nhà hàng thành công.");
        }


        //=== Deactivate Restaurant ===//

        [HttpPost("{restaurantId:guid}/deactivate")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeactivateRestaurant(Guid restaurantId)
        {
            await _platformRestaurantService
                .DeactivateRestaurantAsync(
                    restaurantId);

            return this.ApiOk<object>(
                null,
                "Ngừng hoạt động nhà hàng thành công.");
        }
    }
}

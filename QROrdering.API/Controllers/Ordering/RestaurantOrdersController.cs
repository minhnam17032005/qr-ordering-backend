using Microsoft.AspNetCore.Mvc;
using QROrdering.API.Extensions;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.Ordering.Orders.DTOs.Requests;
using QROrdering.Application.Ordering.Orders.DTOs.Responses;
using QROrdering.Application.Ordering.Orders.Interfaces;
using QROrdering.Domain.Enums;

namespace QROrdering.API.Controllers.Ordering
{
    [ApiController]
    [Route("api/restaurants/{restaurantId:guid}")]
    public class RestaurantOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public RestaurantOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("orders")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<OrderListResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PagedResponse<OrderListResponse>>>> GetOrders(
            Guid restaurantId,
            [FromQuery] PagedRequest request,
            [FromQuery] OrderStatus? status)
        {
            var response = await _orderService.GetRestaurantOrdersAsync(
                restaurantId,
                status,
                request);

            return this.ApiOk(response, "Lấy danh sách đơn hàng thành công.");
        }

        [HttpGet("orders/{orderId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<OrderDetailResponse>>> GetOrderById(
            Guid restaurantId,
            Guid orderId)
        {
            var response = await _orderService.GetRestaurantOrderByIdAsync(
                restaurantId,
                orderId);

            return this.ApiOk(response, "Lấy thông tin đơn hàng thành công.");
        }

        [HttpPatch("orders/{orderId:guid}/status")]
        [ProducesResponseType(typeof(ApiResponse<OrderStatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<OrderStatusResponse>>> UpdateOrderStatus(
            Guid restaurantId,
            Guid orderId,
            [FromBody] OrderStatusUpdateRequest request)
        {
            var response = await _orderService.UpdateOrderStatusAsync(
                restaurantId,
                orderId,
                request);

            return this.ApiOk(response, "Cập nhật trạng thái đơn hàng thành công.");
        }

        [HttpPatch("order-items/{orderItemId:guid}/status")]
        [ProducesResponseType(typeof(ApiResponse<OrderItemStatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<OrderItemStatusResponse>>> UpdateOrderItemStatus(
            Guid restaurantId,
            Guid orderItemId,
            [FromBody] OrderItemStatusUpdateRequest request)
        {
            var response = await _orderService.UpdateOrderItemStatusAsync(
                restaurantId,
                orderItemId,
                request);

            return this.ApiOk(response, "Cập nhật trạng thái món thành công.");
        }

        [HttpPatch("orders/{orderId:guid}/cancel-uncompleted-items")]
        [ProducesResponseType(typeof(ApiResponse<OrderStatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<OrderStatusResponse>>> CancelUncompletedItems(
            Guid restaurantId,
            Guid orderId)
        {
            var response = await _orderService.CancelUncompletedItemsAsync(
                restaurantId,
                orderId);

            return this.ApiOk(
                response,
                "Hủy các món chưa hoàn thành thành công.");
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using QROrdering.API.Extensions;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.Ordering.Orders.DTOs.Requests;
using QROrdering.Application.Ordering.Orders.DTOs.Responses;
using QROrdering.Application.Ordering.Orders.Interfaces;

namespace QROrdering.API.Controllers.Ordering
{
    [ApiController]
    [Route("api/customer/orders")]
    public class CustomerOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public CustomerOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<OrderDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<OrderDetailResponse>>> CreateOrder(
            [FromBody] OrderCreateRequest request,
            [FromHeader(Name = "X-Session-Token")] string sessionToken)
        {
            var response = await _orderService.CreateOrderAsync(
                request,
                sessionToken);

            return this.ApiOk(response, "Tạo đơn hàng thành công.");
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<OrderListResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PagedResponse<OrderListResponse>>>> GetOrders(
            [FromQuery] PagedRequest request,
            [FromHeader(Name = "X-Session-Token")] string sessionToken)
        {
            var response = await _orderService.GetCustomerOrdersAsync(
                sessionToken,
                request);

            return this.ApiOk(response, "Lấy danh sách đơn hàng thành công.");
        }

        [HttpGet("{orderId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<OrderDetailResponse>>> GetOrderById(
            Guid orderId,
            [FromHeader(Name = "X-Session-Token")] string sessionToken)
        {
            var response = await _orderService.GetCustomerOrderByIdAsync(
                sessionToken,
                orderId);

            return this.ApiOk(response, "Lấy thông tin đơn hàng thành công.");
        }
    }
}
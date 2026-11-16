using Microsoft.AspNetCore.Mvc;
using QROrdering.API.Extensions;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.Ordering.CustomerSessions.DTOs.Responses;
using QROrdering.Application.Ordering.CustomerSessions.Interfaces;

namespace QROrdering.API.Controllers.Ordering
{
    [ApiController]
    [Route("api/restaurants/{restaurantId:guid}/customer-sessions")]
    public class RestaurantCustomerSessionsController : ControllerBase
    {
        private readonly ICustomerSessionService _sessionService;

        public RestaurantCustomerSessionsController(
            ICustomerSessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<CustomerSessionListResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PagedResponse<CustomerSessionListResponse>>>> GetSessions(
            Guid restaurantId,
            [FromQuery] PagedRequest request)
        {
            var response = await _sessionService.GetSessionsAsync(
                restaurantId,
                request);

            return this.ApiOk(response, "Lấy danh sách phiên khách hàng thành công.");
        }

        [HttpGet("{sessionId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerSessionDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CustomerSessionDetailResponse>>> GetSessionById(
            Guid restaurantId,
            Guid sessionId)
        {
            var response = await _sessionService.GetSessionByIdAsync(
                restaurantId,
                sessionId);

            return this.ApiOk(response, "Lấy thông tin phiên khách hàng thành công.");
        }
    }
}
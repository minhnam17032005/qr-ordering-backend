using Microsoft.AspNetCore.Mvc;
using QROrdering.API.Extensions;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.Ordering.CustomerSessions.DTOs.Requests;
using QROrdering.Application.Ordering.CustomerSessions.DTOs.Responses;
using QROrdering.Application.Ordering.CustomerSessions.Interfaces;

namespace QROrdering.API.Controllers.Ordering
{
    [ApiController]
    public class CustomerSessionsController : ControllerBase
    {
        private readonly ICustomerSessionService _sessionService;

        public CustomerSessionsController(
            ICustomerSessionService sessionService)
        {
            _sessionService = sessionService;
        }

        // ============================================================
        // CUSTOMER APIs
        // ============================================================

        [HttpPost("api/customer-sessions/scan")]
        [ProducesResponseType(typeof(ApiResponse<CustomerSessionCreatedResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<CustomerSessionCreatedResponse>>> ScanTable(
            [FromBody] ScanTableRequest request)
        {
            var response = await _sessionService.ScanTableAsync(request);

            return this.ApiOk(response, "Tạo phiên khách hàng thành công.");
        }

        [HttpGet("api/customer-sessions/current")]
        [ProducesResponseType(typeof(ApiResponse<CustomerSessionDetailResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CustomerSessionDetailResponse>>> GetCurrentSession(
            [FromHeader(Name = "X-Session-Token")] string sessionToken)
        {
            var response = await _sessionService.GetCurrentSessionAsync(
                sessionToken);

            return this.ApiOk(response, "Lấy phiên khách hàng thành công.");
        }

        // ============================================================
        // RESTAURANT MANAGEMENT APIs
        // ============================================================

        [HttpGet("api/restaurants/{restaurantId:guid}/customer-sessions")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<CustomerSessionListResponse>>),StatusCodes.Status200OK)]
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

        [HttpGet("api/restaurants/{restaurantId:guid}/customer-sessions/{sessionId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerSessionDetailResponse>),StatusCodes.Status200OK)]
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
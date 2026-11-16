using Microsoft.AspNetCore.Mvc;
using QROrdering.API.Extensions;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.Ordering.CustomerSessions.DTOs.Requests;
using QROrdering.Application.Ordering.CustomerSessions.DTOs.Responses;
using QROrdering.Application.Ordering.CustomerSessions.Interfaces;

namespace QROrdering.API.Controllers.Ordering
{
    [ApiController]
    [Route("api/customer-sessions")]
    public class CustomerSessionsController : ControllerBase
    {
        private readonly ICustomerSessionService _sessionService;

        public CustomerSessionsController(
            ICustomerSessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpPost("scan")]
        [ProducesResponseType(typeof(ApiResponse<CustomerSessionCreatedResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<CustomerSessionCreatedResponse>>> ScanTable(
            [FromBody] ScanTableRequest request)
        {
            var response = await _sessionService.ScanTableAsync(request);

            return this.ApiOk(response, "Tạo phiên khách hàng thành công.");
        }

        [HttpGet("current")]
        [ProducesResponseType(typeof(ApiResponse<CustomerSessionDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CustomerSessionDetailResponse>>> GetCurrentSession(
            [FromHeader(Name = "X-Session-Token")] string sessionToken)
        {
            var response = await _sessionService.GetCurrentSessionAsync(sessionToken);

            return this.ApiOk(response, "Lấy phiên khách hàng thành công.");
        }
    }
}
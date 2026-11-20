using Microsoft.AspNetCore.Mvc;
using QROrdering.API.Extensions;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.Ordering.Payments.DTOs.Requests;
using QROrdering.Application.Ordering.Payments.DTOs.Responses;
using QROrdering.Application.Ordering.Payments.Interfaces;
using QROrdering.Domain.Enums;

namespace QROrdering.API.Controllers.Ordering
{
    [ApiController]
    [Route("api/restaurants/{restaurantId:guid}")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // ============================================================
        // CREATE PAYMENT
        // ============================================================

        [HttpPost("customer-sessions/{sessionId:guid}/payments")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> CreatePayment(
            Guid restaurantId,
            Guid sessionId,
            [FromBody] PaymentCreateRequest request)
        {
            var response = await _paymentService.CreatePaymentAsync(
                restaurantId,
                sessionId,
                request);

            return this.ApiOk(
                response,
                "Tạo giao dịch thanh toán thành công.");
        }

        // ============================================================
        // GET PAYMENTS
        // ============================================================

        [HttpGet("payments")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<PaymentListResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PagedResponse<PaymentListResponse>>>> GetPayments(
            Guid restaurantId,
            [FromQuery] PagedRequest request,
            [FromQuery] PaymentStatus? status)
        {
            var response = await _paymentService.GetPaymentsAsync(
                restaurantId,
                status,
                request);

            return this.ApiOk(
                response,
                "Lấy danh sách giao dịch thành công.");
        }

        // ============================================================
        // GET PAYMENT BY ID
        // ============================================================

        [HttpGet("payments/{paymentId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> GetPaymentById(
            Guid restaurantId,
            Guid paymentId)
        {
            var response = await _paymentService.GetPaymentByIdAsync(
                restaurantId,
                paymentId);

            return this.ApiOk(
                response,
                "Lấy thông tin giao dịch thành công.");
        }

        // ============================================================
        // CONFIRM PAYMENT
        // ============================================================

        [HttpPatch("payments/{paymentId:guid}/confirm")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> ConfirmPayment(
            Guid restaurantId,
            Guid paymentId,
            [FromBody] PaymentConfirmRequest request)
        {
            var response = await _paymentService.ConfirmPaymentAsync(
                restaurantId,
                paymentId,
                request);

            return this.ApiOk(
                response,
                "Xác nhận thanh toán thành công.");
        }

        // ============================================================
        // RETRY PAYMENT
        // ============================================================

        [HttpPost("payments/{paymentId:guid}/retry")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> RetryPayment(
            Guid restaurantId,
            Guid paymentId,
            [FromBody] PaymentCreateRequest request)
        {
            var response = await _paymentService.RetryPaymentAsync(
                restaurantId,
                paymentId,
                request);

            return this.ApiOk(
                response,
                "Tạo giao dịch thanh toán mới thành công.");
        }
    }
}
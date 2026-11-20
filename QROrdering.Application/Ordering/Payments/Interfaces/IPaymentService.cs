using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Ordering.Payments.DTOs.Requests;
using QROrdering.Application.Ordering.Payments.DTOs.Responses;
using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.Payments.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentAsync(
            Guid restaurantId,
            Guid customerSessionId,
            PaymentCreateRequest request);

        Task<PagedResponse<PaymentListResponse>> GetPaymentsAsync(
            Guid restaurantId,
            PaymentStatus? status,
            PagedRequest request);

        Task<PaymentResponse> GetPaymentByIdAsync(
            Guid restaurantId,
            Guid paymentId);

        Task<PaymentResponse> ConfirmPaymentAsync(
            Guid restaurantId,
            Guid paymentId,
            PaymentConfirmRequest request);

        Task<PaymentResponse> RetryPaymentAsync(
            Guid restaurantId,
            Guid paymentId,
            PaymentCreateRequest request);
    }
}
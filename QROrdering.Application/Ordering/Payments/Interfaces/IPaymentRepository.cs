using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Ordering.Payments.DTOs.Responses;
using QROrdering.Domain.Entities.Ordering;
using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.Payments.Interfaces
{
    public interface IPaymentRepository
    {
        Task<bool> RestaurantExistsAsync(Guid restaurantId);

        Task<int> CountByRestaurantAsync(
            Guid restaurantId,
            PaymentStatus? status);

        Task<List<PaymentListResponse>> GetPagedByRestaurantAsync(
            Guid restaurantId,
            PaymentStatus? status,
            PagedRequest request);

        Task<PaymentResponse?> GetByIdInRestaurantAsync(
            Guid restaurantId,
            Guid paymentId);

        Task<Payment?> GetEntityByIdAsync(
            Guid restaurantId,
            Guid paymentId);

        Task<Guid?> GetSessionIdByPaymentIdAsync(
            Guid restaurantId,
            Guid paymentId);

        Task<bool> HasPendingBySessionAsync(
            Guid restaurantId,
            Guid customerSessionId);

        Task<decimal> CalculatePayableAmountAsync(
            Guid restaurantId,
            Guid customerSessionId);

        Task AddAsync(Payment payment);

        Task FailPendingBySessionAsync(
            Guid restaurantId,
            Guid customerSessionId,
            DateTime now);
    }
}
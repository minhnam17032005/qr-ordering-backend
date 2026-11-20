using Microsoft.EntityFrameworkCore;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Ordering.Payments.DTOs.Responses;
using QROrdering.Application.Ordering.Payments.Interfaces;
using QROrdering.Domain.Entities.Ordering;
using QROrdering.Domain.Enums;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Ordering.Payments
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly QROrderingDbContext _context;

        public PaymentRepository(QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RestaurantExistsAsync(Guid restaurantId)
        {
            return await _context.Restaurants
                .AnyAsync(x => x.Id == restaurantId);
        }

        public async Task<int> CountByRestaurantAsync(
            Guid restaurantId,
            PaymentStatus? status)
        {
            var query = _context.Payments
                .Where(x => x.RestaurantId == restaurantId);

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            return await query.CountAsync();
        }

        public async Task<List<PaymentListResponse>> GetPagedByRestaurantAsync(
            Guid restaurantId,
            PaymentStatus? status,
            PagedRequest request)
        {
            var query = _context.Payments
                .AsNoTracking()
                .Where(x => x.RestaurantId == restaurantId);

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            var skip = (long)(request.Page - 1) * request.PageSize;

            if (skip > int.MaxValue)
            {
                return [];
            }

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .Skip((int)skip)
                .Take(request.PageSize)
                .Select(x => new PaymentListResponse
                {
                    Id = x.Id,
                    CustomerSessionId = x.CustomerSessionId,
                    TableNumber = x.CustomerSession.RestaurantTable.TableNumber,
                    PaymentMethod = x.PaymentMethod,
                    Status = x.Status,
                    Amount = x.Amount,
                    CreatedAt = x.CreatedAt,
                    PaidAt = x.PaidAt
                })
                .ToListAsync();
        }

        public async Task<PaymentResponse?> GetByIdInRestaurantAsync(
            Guid restaurantId,
            Guid paymentId)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(x =>
                    x.RestaurantId == restaurantId &&
                    x.Id == paymentId)
                .Select(x => new PaymentResponse
                {
                    Id = x.Id,
                    RestaurantId = x.RestaurantId,
                    CustomerSessionId = x.CustomerSessionId,
                    TableNumber = x.CustomerSession.RestaurantTable.TableNumber,
                    PaymentMethod = x.PaymentMethod,
                    Status = x.Status,
                    Amount = x.Amount,
                    TransactionCode = x.TransactionCode,
                    PaidAt = x.PaidAt,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Payment?> GetEntityByIdAsync(
            Guid restaurantId,
            Guid paymentId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(x =>
                    x.RestaurantId == restaurantId &&
                    x.Id == paymentId);
        }

        public async Task<Guid?> GetSessionIdByPaymentIdAsync(
            Guid restaurantId,
            Guid paymentId)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(x =>
                    x.RestaurantId == restaurantId &&
                    x.Id == paymentId)
                .Select(x => (Guid?)x.CustomerSessionId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasPendingBySessionAsync(
            Guid restaurantId,
            Guid customerSessionId)
        {
            return await _context.Payments
                .AnyAsync(x =>
                    x.RestaurantId == restaurantId &&
                    x.CustomerSessionId == customerSessionId &&
                    x.Status == PaymentStatus.Pending);
        }

        public async Task<decimal> CalculatePayableAmountAsync(
            Guid restaurantId,
            Guid customerSessionId)
        {
            return await _context.OrderItems
                .Where(x =>
                    x.RestaurantId == restaurantId &&
                    x.Order.CustomerSessionId == customerSessionId &&
                    x.Status != OrderItemStatus.Cancelled)
                .SumAsync(x =>
                    (decimal?)(x.Quantity * x.UnitPrice)) ?? 0m;
        }

        public Task AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            return Task.CompletedTask;
        }

        public async Task FailPendingBySessionAsync(
            Guid restaurantId,
            Guid customerSessionId,
            DateTime now)
        {
            var payments = await _context.Payments
                .Where(x =>
                    x.RestaurantId == restaurantId &&
                    x.CustomerSessionId == customerSessionId &&
                    x.Status == PaymentStatus.Pending)
                .ToListAsync();

            foreach (var payment in payments)
            {
                payment.Status = PaymentStatus.Failed;
                payment.UpdatedAt = now;
            }
        }
    }
}
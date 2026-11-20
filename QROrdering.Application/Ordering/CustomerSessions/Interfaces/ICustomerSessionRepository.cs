using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Ordering.CustomerSessions.DTOs.Responses;
using QROrdering.Domain.Entities.Ordering;
using QROrdering.Domain.Entities.RestaurantManagement;

namespace QROrdering.Application.Ordering.CustomerSessions.Interfaces
{
    public interface ICustomerSessionRepository
    {
        Task<bool> RestaurantExistsAsync(Guid restaurantId);

        Task<bool> RestaurantIsActiveAsync(Guid restaurantId);

        Task<RestaurantTable?> GetTableForScanAsync(
            Guid restaurantId,
            Guid tableId);

        Task<bool> HasActiveSessionAsync(
            Guid restaurantId,
            Guid tableId);

        Task<int> CountByRestaurantAsync(Guid restaurantId);

        Task<List<CustomerSessionListResponse>> GetPagedByRestaurantAsync(
            Guid restaurantId,
            PagedRequest request);

        Task<CustomerSessionDetailResponse?> GetByIdInRestaurantAsync(
            Guid restaurantId,
            Guid sessionId);

        Task<CustomerSessionDetailResponse?> GetByTokenHashAsync(
            string tokenHash);

        Task AddAsync(CustomerSession session);

        Task<CustomerSession?> GetByIdForUpdateAsync(
            Guid restaurantId,
            Guid sessionId);
    }
}
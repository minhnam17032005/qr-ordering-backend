using QROrdering.Application.Common.Pagination;
using QROrdering.Application.RestaurantManagement.Tables.DTOs.Responses;
using QROrdering.Domain.Entities.RestaurantManagement;

namespace QROrdering.Application.RestaurantManagement.Tables.Interfaces
{
    public interface IRestaurantTableRepository
    {
        Task<bool> RestaurantExistsAsync(Guid restaurantId);

        Task<int> CountByRestaurantAsync(Guid restaurantId);

        Task<List<TableListResponse>> GetPagedByRestaurantAsync(
            Guid restaurantId,
            PagedRequest request);

        Task<TableDetailResponse?> GetByIdInRestaurantAsync(
            Guid restaurantId,
            Guid tableId);

        Task<bool> ExistsByTableNumberAsync(
            Guid restaurantId,
            int tableNumber,
            Guid? excludeTableId = null);

        Task<RestaurantTable?> GetEntityByIdInRestaurantAsync(
            Guid restaurantId,
            Guid tableId);

        Task AddAsync(RestaurantTable table);
    }
}
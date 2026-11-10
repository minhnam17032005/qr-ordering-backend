using QROrdering.Application.Common.Pagination;
using QROrdering.Application.RestaurantManagement.Tables.DTOs.Requests;
using QROrdering.Application.RestaurantManagement.Tables.DTOs.Responses;

namespace QROrdering.Application.RestaurantManagement.Tables.Interfaces
{
    public interface IRestaurantTableService
    {
        Task<PagedResponse<TableListResponse>> GetTablesAsync(
            Guid restaurantId,
            PagedRequest request);

        Task<TableDetailResponse> GetTableByIdAsync(
            Guid restaurantId,
            Guid tableId);

        Task<TableDetailResponse> CreateTableAsync(
            Guid restaurantId,
            TableCreateRequest request);

        Task<TableDetailResponse> UpdateTableAsync(
            Guid restaurantId,
            Guid tableId,
            TableUpdateRequest request);

        Task<TableStatusResponse> ActivateTableAsync(
            Guid restaurantId,
            Guid tableId);

        Task<TableStatusResponse> DeactivateTableAsync(
            Guid restaurantId,
            Guid tableId);
    }
}
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.RestaurantManagement.Categories.DTOs.Responses;
using QROrdering.Domain.Entities.RestaurantManagement;

namespace QROrdering.Application.RestaurantManagement.Categories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<bool> RestaurantExistsAsync(Guid restaurantId);

        Task<int> CountByRestaurantAsync(
            Guid restaurantId,
            string? search);

        Task<List<CategoryListResponse>> GetPagedByRestaurantAsync(
            Guid restaurantId,
            PagedRequest request);

        Task<CategoryDetailResponse?> GetByIdInRestaurantAsync(
            Guid restaurantId,
            Guid categoryId);

        Task<bool> ExistsByNameAsync(
           Guid restaurantId,
           string name,
           Guid? excludeCategoryId = null);

        Task<Category?> GetEntityByIdInRestaurantAsync(
            Guid restaurantId,
            Guid categoryId);

        Task AddAsync(Category category);
    }
}
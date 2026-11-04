using QROrdering.Application.Common.Pagination;
using QROrdering.Application.RestaurantManagement.Categories.DTOs.Requests;
using QROrdering.Application.RestaurantManagement.Categories.DTOs.Responses;

namespace QROrdering.Application.RestaurantManagement.Categories.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedResponse<CategoryListResponse>> GetCategoriesAsync(
            Guid restaurantId,
            PagedRequest request);

        Task<CategoryDetailResponse> GetCategoryByIdAsync(
            Guid restaurantId,
            Guid categoryId);
        Task<CategoryDetailResponse> CreateCategoryAsync(
           Guid restaurantId,
           CategoryCreateRequest request);

        Task<CategoryDetailResponse> UpdateCategoryAsync(
            Guid restaurantId,
            Guid categoryId,
            CategoryUpdateRequest request);

        Task<CategoryStatusResponse> ActivateCategoryAsync(
            Guid restaurantId,
            Guid categoryId);

        Task<CategoryStatusResponse> DeactivateCategoryAsync(
            Guid restaurantId,
            Guid categoryId);
    }
}
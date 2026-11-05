using QROrdering.Application.Common.Pagination;
using QROrdering.Application.RestaurantManagement.Products.DTOs.Responses;
using QROrdering.Domain.Entities.RestaurantManagement;

namespace QROrdering.Application.RestaurantManagement.Products.Interfaces
{
    public interface IProductRepository
    {
        Task<bool> RestaurantExistsAsync(Guid restaurantId);

        Task<bool> CategoryExistsInRestaurantAsync(
            Guid restaurantId,
            Guid categoryId);

        Task<int> CountByRestaurantAsync(
            Guid restaurantId,
            Guid? categoryId);

        Task<List<ProductListResponse>> GetPagedByRestaurantAsync(
            Guid restaurantId,
            Guid? categoryId,
            PagedRequest request);

        Task<ProductDetailResponse?> GetByIdInRestaurantAsync(
            Guid restaurantId,
            Guid productId);

        Task<bool> ExistsByNameAsync(
            Guid restaurantId,
            string name,
            Guid? excludeProductId = null);

        Task<Product?> GetEntityByIdInRestaurantAsync(
            Guid restaurantId,
            Guid productId);

        Task AddAsync(Product product);
    }
}
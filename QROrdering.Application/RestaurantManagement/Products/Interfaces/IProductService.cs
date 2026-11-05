using QROrdering.Application.Common.Pagination;
using QROrdering.Application.RestaurantManagement.Products.DTOs.Requests;
using QROrdering.Application.RestaurantManagement.Products.DTOs.Responses;

namespace QROrdering.Application.RestaurantManagement.Products.Interfaces
{
    public interface IProductService
    {
        Task<PagedResponse<ProductListResponse>> GetProductsAsync(
            Guid restaurantId,
            Guid? categoryId,
            PagedRequest request);

        Task<ProductDetailResponse> GetProductByIdAsync(
            Guid restaurantId,
            Guid productId);

        Task<ProductDetailResponse> CreateProductAsync(
            Guid restaurantId,
            ProductCreateRequest request);

        Task<ProductDetailResponse> UpdateProductAsync(
            Guid restaurantId,
            Guid productId,
            ProductUpdateRequest request);
        Task<ProductStatusResponse> ActivateProductAsync(
            Guid restaurantId,
            Guid productId);

        Task<ProductStatusResponse> DeactivateProductAsync(
            Guid restaurantId,
            Guid productId);
    }
}
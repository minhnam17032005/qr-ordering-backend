using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Exceptions;
using QROrdering.Application.RestaurantManagement.Products.DTOs.Requests;
using QROrdering.Application.RestaurantManagement.Products.DTOs.Responses;
using QROrdering.Application.RestaurantManagement.Products.Interfaces;
using QROrdering.Domain.Entities.RestaurantManagement;

namespace QROrdering.Application.RestaurantManagement.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResponse<ProductListResponse>> GetProductsAsync(
            Guid restaurantId,
            Guid? categoryId,
            PagedRequest request)
        {
            // Validate request
            if (restaurantId == Guid.Empty)
            {
                throw new BadRequestException("RestaurantId không hợp lệ.");
            }

            // Validate Restaurant
            var restaurantExists =
                await _productRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
            {
                throw new NotFoundException("Không tìm thấy nhà hàng.");
            }

            // Validate Category when provided
            if (categoryId.HasValue)
            {
                if (categoryId.Value == Guid.Empty)
                {
                    throw new BadRequestException("CategoryId không hợp lệ.");
                }

                var categoryExists =
                    await _productRepository.CategoryExistsInRestaurantAsync(
                        restaurantId,
                        categoryId.Value);

                if (!categoryExists)
                {
                    throw new NotFoundException("Không tìm thấy danh mục.");
                }
            }

            // Count
            var totalItems = await _productRepository.CountByRestaurantAsync(
                restaurantId,
                categoryId);

            // Get paged products
            var products = await _productRepository.GetPagedByRestaurantAsync(
                restaurantId,
                categoryId,
                request);

            return new PagedResponse<ProductListResponse>
            {
                Items = products,
                Pagination = new PaginationMeta
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling(
                        (double)totalItems / request.PageSize)
                }
            };
        }

        public async Task<ProductDetailResponse> GetProductByIdAsync(
            Guid restaurantId,
            Guid productId)
        {
            if (restaurantId == Guid.Empty)
            {
                throw new BadRequestException("RestaurantId không hợp lệ.");
            }

            if (productId == Guid.Empty)
            {
                throw new BadRequestException("ProductId không hợp lệ.");
            }

            var restaurantExists =
                await _productRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
            {
                throw new NotFoundException("Không tìm thấy nhà hàng.");
            }

            var product = await _productRepository.GetByIdInRestaurantAsync(
                restaurantId,
                productId);

            if (product is null)
            {
                throw new NotFoundException("Không tìm thấy sản phẩm.");
            }

            return product;
        }

        public async Task<ProductDetailResponse> CreateProductAsync(
            Guid restaurantId,
            ProductCreateRequest request)
        {
            if (restaurantId == Guid.Empty)
            {
                throw new BadRequestException("RestaurantId không hợp lệ.");
            }

            if (request.CategoryId == Guid.Empty)
            {
                throw new BadRequestException("CategoryId không hợp lệ.");
            }

            var restaurantExists =
                await _productRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
            {
                throw new NotFoundException("Không tìm thấy nhà hàng.");
            }

            var categoryExists =
                await _productRepository.CategoryExistsInRestaurantAsync(
                    restaurantId,
                    request.CategoryId);

            if (!categoryExists)
            {
                throw new NotFoundException("Không tìm thấy danh mục.");
            }

            var name = request.Name.Trim();
            var description = NormalizeOptionalString(request.Description);
            var imageUrl = NormalizeOptionalString(request.ImageUrl);

            var nameExists = await _productRepository.ExistsByNameAsync(
                restaurantId,
                name);

            if (nameExists)
            {
                throw new BadRequestException("Tên sản phẩm đã tồn tại.");
            }

            var product = new Product
            {
                RestaurantId = restaurantId,
                CategoryId = request.CategoryId,
                Name = name,
                Description = description,
                Price = request.Price,
                ImageUrl = imageUrl,
                IsActive = true
            };

            await _productRepository.AddAsync(product);

            await _unitOfWork.SaveChangesAsync();

            var response = await _productRepository.GetByIdInRestaurantAsync(
                restaurantId,
                product.Id);

            if (response is null)
            {
                throw new NotFoundException("Không tìm thấy sản phẩm sau khi tạo.");
            }

            return response;
        }

        public async Task<ProductDetailResponse> UpdateProductAsync(
            Guid restaurantId,
            Guid productId,
            ProductUpdateRequest request)
        {
            if (restaurantId == Guid.Empty || productId == Guid.Empty)
            {
                throw new BadRequestException("Thông tin định danh không hợp lệ.");
            }

            if (request.CategoryId == Guid.Empty)
            {
                throw new BadRequestException("CategoryId không hợp lệ.");
            }

            var restaurantExists =
                await _productRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
            {
                throw new NotFoundException("Không tìm thấy nhà hàng.");
            }

            var product = await _productRepository.GetEntityByIdInRestaurantAsync(
                restaurantId,
                productId);

            if (product is null)
            {
                throw new NotFoundException("Không tìm thấy sản phẩm.");
            }

            var categoryExists =
                await _productRepository.CategoryExistsInRestaurantAsync(
                    restaurantId,
                    request.CategoryId);

            if (!categoryExists)
            {
                throw new NotFoundException("Không tìm thấy danh mục.");
            }

            var name = request.Name.Trim();
            var description = NormalizeOptionalString(request.Description);
            var imageUrl = NormalizeOptionalString(request.ImageUrl);

            var nameExists = await _productRepository.ExistsByNameAsync(
                restaurantId,
                name,
                productId);

            if (nameExists)
            {
                throw new BadRequestException("Tên sản phẩm đã tồn tại.");
            }

            product.CategoryId = request.CategoryId;
            product.Name = name;
            product.Description = description;
            product.Price = request.Price;
            product.ImageUrl = imageUrl;
            product.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            var response = await _productRepository.GetByIdInRestaurantAsync(
                restaurantId,
                productId);

            if (response is null)
            {
                throw new NotFoundException("Không tìm thấy sản phẩm sau khi cập nhật.");
            }

            return response;
        }

        public async Task<ProductStatusResponse> ActivateProductAsync(
    Guid restaurantId,
    Guid productId)
        {
            if (restaurantId == Guid.Empty || productId == Guid.Empty)
                throw new BadRequestException("Thông tin không hợp lệ.");

            var restaurantExists =
                await _productRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
                throw new NotFoundException("Không tìm thấy nhà hàng.");

            var product = await _productRepository.GetEntityByIdInRestaurantAsync(
                restaurantId,
                productId);

            if (product is null)
                throw new NotFoundException("Không tìm thấy sản phẩm.");

            if (product.IsActive)
                throw new BadRequestException("Sản phẩm đã được kích hoạt.");

            product.IsActive = true;
            product.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return new ProductStatusResponse
            {
                Id = product.Id,
                IsActive = product.IsActive
            };
        }

        public async Task<ProductStatusResponse> DeactivateProductAsync(
            Guid restaurantId,
            Guid productId)
        {
            if (restaurantId == Guid.Empty || productId == Guid.Empty)
                throw new BadRequestException("Thông tin không hợp lệ.");

            var restaurantExists =
                await _productRepository.RestaurantExistsAsync(restaurantId);

            if (!restaurantExists)
                throw new NotFoundException("Không tìm thấy nhà hàng.");

            var product = await _productRepository.GetEntityByIdInRestaurantAsync(
                restaurantId,
                productId);

            if (product is null)
                throw new NotFoundException("Không tìm thấy sản phẩm.");

            if (!product.IsActive)
                throw new BadRequestException("Sản phẩm đã bị ngừng kinh doanh.");

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return new ProductStatusResponse
            {
                Id = product.Id,
                IsActive = product.IsActive
            };
        }

        private static string? NormalizeOptionalString(string? value)
        {
            var normalized = value?.Trim();

            return string.IsNullOrWhiteSpace(normalized)
                ? null
                : normalized;
        }
    }
}

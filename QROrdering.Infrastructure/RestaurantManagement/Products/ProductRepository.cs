using Microsoft.EntityFrameworkCore;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.RestaurantManagement.Products.DTOs.Responses;
using QROrdering.Application.RestaurantManagement.Products.Interfaces;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.RestaurantManagement.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly QROrderingDbContext _context;

        public ProductRepository(QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RestaurantExistsAsync(Guid restaurantId)
        {
            return await _context.Restaurants
                .AnyAsync(x => x.Id == restaurantId);
        }

        public async Task<bool> CategoryExistsInRestaurantAsync(
            Guid restaurantId,
            Guid categoryId)
        {
            return await _context.Categories
                .AnyAsync(x =>
                    x.RestaurantId == restaurantId &&
                    x.Id == categoryId);
        }

        public async Task<int> CountByRestaurantAsync(
            Guid restaurantId,
            Guid? categoryId)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(x => x.RestaurantId == restaurantId);

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<List<ProductListResponse>> GetPagedByRestaurantAsync(
            Guid restaurantId,
            Guid? categoryId,
            PagedRequest request)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(x => x.RestaurantId == restaurantId);

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            query = query
                .OrderByDescending(x => x.IsActive)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.Id);

            var skip = (long)(request.Page - 1) * request.PageSize;

            if (skip > int.MaxValue)
            {
                return [];
            }

            return await query
                .Skip((int)skip)
                .Take(request.PageSize)
                .Select(x => new ProductListResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    ImageUrl = x.ImageUrl,
                    IsActive = x.IsActive,

                    Category = new CategorySummaryResponse
                    {
                        Id = x.Category.Id,
                        Name = x.Category.Name
                    }
                })
                .ToListAsync();
        }

        public async Task<ProductDetailResponse?> GetByIdInRestaurantAsync(
            Guid restaurantId,
            Guid productId)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(x =>
                    x.RestaurantId == restaurantId &&
                    x.Id == productId)
                .Select(x => new ProductDetailResponse
                {
                    Id = x.Id,
                    RestaurantId = x.RestaurantId,
                    CategoryId = x.CategoryId,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    ImageUrl = x.ImageUrl,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsByNameAsync(
            Guid restaurantId,
            string name,
            Guid? excludeProductId = null)
        {
            var query = _context.Products
                .Where(x =>
                    x.RestaurantId == restaurantId &&
                    x.Name.ToLower() == name.ToLower());

            if (excludeProductId.HasValue)
            {
                query = query.Where(x => x.Id != excludeProductId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<Product?> GetEntityByIdInRestaurantAsync(
            Guid restaurantId,
            Guid productId)
        {
            return await _context.Products
                .FirstOrDefaultAsync(x =>
                    x.RestaurantId == restaurantId &&
                    x.Id == productId);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }
    }
}
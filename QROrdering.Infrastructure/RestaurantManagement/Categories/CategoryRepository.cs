
using Microsoft.EntityFrameworkCore;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.RestaurantManagement.Categories.DTOs.Responses;
using QROrdering.Application.RestaurantManagement.Categories.Interfaces;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.RestaurantManagement.Categories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly QROrderingDbContext _context;

        public CategoryRepository(QROrderingDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // CHECK RESTAURANT
        // ============================================================

        public async Task<bool> RestaurantExistsAsync(Guid restaurantId)
        {
            return await _context.Restaurants
                .AsNoTracking()
                .AnyAsync(restaurant => restaurant.Id == restaurantId);
        }

        // ============================================================
        // COUNT CATEGORIES
        // ============================================================

        public async Task<int> CountByRestaurantAsync(
            Guid restaurantId,
            string? search)
        {
            var query = _context.Categories
                .AsNoTracking()
                .Where(category =>
                    category.RestaurantId == restaurantId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLower();

                query = query.Where(category =>
                    category.Name.ToLower().Contains(keyword));
            }

            return await query.CountAsync();
        }

        // ============================================================
        // GET PAGED CATEGORIES
        // ============================================================

        public async Task<List<CategoryListResponse>> GetPagedByRestaurantAsync(
            Guid restaurantId,
            PagedRequest request)
        {
            var query = _context.Categories
                .AsNoTracking()
                .Where(category =>
                    category.RestaurantId == restaurantId);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var keyword = request.Search.Trim().ToLower();

                query = query.Where(category =>
                    category.Name.ToLower().Contains(keyword));
            }

            // Active categories are always displayed first.
            IOrderedQueryable<Category> orderedQuery =
                query.OrderByDescending(category => category.IsActive);

            switch (request.SortBy?.Trim().ToLowerInvariant())
            {
                case "name":
                    orderedQuery = request.SortDescending
                        ? orderedQuery.ThenByDescending(category => category.Name)
                        : orderedQuery.ThenBy(category => category.Name);
                    break;

                case "createdat":
                    orderedQuery = request.SortDescending
                        ? orderedQuery.ThenByDescending(category => category.CreatedAt)
                        : orderedQuery.ThenBy(category => category.CreatedAt);
                    break;

                case "updatedat":
                    orderedQuery = request.SortDescending
                        ? orderedQuery.ThenByDescending(category => category.UpdatedAt)
                        : orderedQuery.ThenBy(category => category.UpdatedAt);
                    break;

                default:
                    orderedQuery = orderedQuery
                        .ThenBy(category => category.Name);
                    break;
            }

            return await orderedQuery
                .ThenBy(category => category.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(category => new CategoryListResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    ImageUrl = category.ImageUrl,
                    IsActive = category.IsActive,
                    ProductCount = category.Products.Count()
                })
                .ToListAsync();
        }

        // ============================================================
        // GET CATEGORY BY ID
        // ============================================================

        public async Task<CategoryDetailResponse?> GetByIdInRestaurantAsync(
            Guid restaurantId,
            Guid categoryId)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(category =>
                    category.Id == categoryId &&
                    category.RestaurantId == restaurantId)
                .Select(category => new CategoryDetailResponse
                {
                    Id = category.Id,
                    RestaurantId = category.RestaurantId,
                    Name = category.Name,
                    Description = category.Description,
                    ImageUrl = category.ImageUrl,
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsByNameAsync(
        Guid restaurantId,
        string name,
        Guid? excludeCategoryId = null)
        {
            var query = _context.Categories
                .AsNoTracking()
                .Where(category =>
                    category.RestaurantId == restaurantId &&
                    category.Name.ToLower() == name.ToLower());

            if (excludeCategoryId.HasValue)
            {
                query = query.Where(category =>
                    category.Id != excludeCategoryId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<Category?> GetEntityByIdInRestaurantAsync(
            Guid restaurantId,
            Guid categoryId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(category =>
                    category.Id == categoryId &&
                    category.RestaurantId == restaurantId);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.RestaurantManagement.Tables.DTOs.Responses;
using QROrdering.Application.RestaurantManagement.Tables.Interfaces;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.RestaurantManagement.Tables
{
    public class RestaurantTableRepository : IRestaurantTableRepository
    {
        private readonly QROrderingDbContext _context;

        public RestaurantTableRepository(QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RestaurantExistsAsync(Guid restaurantId)
        {
            return await _context.Restaurants
                .AnyAsync(r => r.Id == restaurantId);
        }

        public async Task<int> CountByRestaurantAsync(Guid restaurantId)
        {
            return await _context.RestaurantTables
                .CountAsync(t => t.RestaurantId == restaurantId);
        }

        public async Task<List<TableListResponse>> GetPagedByRestaurantAsync(
            Guid restaurantId,
            PagedRequest request)
        {
            return await _context.RestaurantTables
                .AsNoTracking()
                .Where(t => t.RestaurantId == restaurantId)
                .OrderBy(t => t.TableNumber)
                .ThenBy(t => t.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new TableListResponse
                {
                    Id = t.Id,
                    TableNumber = t.TableNumber,
                    Status = t.Status,
                    IsActive = t.IsActive
                })
                .ToListAsync();
        }

        public async Task<TableDetailResponse?> GetByIdInRestaurantAsync(
            Guid restaurantId,
            Guid tableId)
        {
            return await _context.RestaurantTables
                .AsNoTracking()
                .Where(t =>
                    t.RestaurantId == restaurantId &&
                    t.Id == tableId)
                .Select(t => new TableDetailResponse
                {
                    Id = t.Id,
                    RestaurantId = t.RestaurantId,
                    TableNumber = t.TableNumber,
                    QRCode = t.QRCode,
                    Status = t.Status,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsByTableNumberAsync(
            Guid restaurantId,
            int tableNumber,
            Guid? excludeTableId = null)
        {
            var query = _context.RestaurantTables
                .Where(t =>
                    t.RestaurantId == restaurantId &&
                    t.TableNumber == tableNumber);

            if (excludeTableId.HasValue)
            {
                query = query.Where(t => t.Id != excludeTableId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<RestaurantTable?> GetEntityByIdInRestaurantAsync(
            Guid restaurantId,
            Guid tableId)
        {
            return await _context.RestaurantTables
                .FirstOrDefaultAsync(t =>
                    t.RestaurantId == restaurantId &&
                    t.Id == tableId);
        }

        public Task AddAsync(RestaurantTable table)
        {
            _context.RestaurantTables.Add(table);
            return Task.CompletedTask;
        }
    }
}
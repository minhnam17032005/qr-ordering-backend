using Microsoft.EntityFrameworkCore;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Ordering.CustomerSessions.DTOs.Responses;
using QROrdering.Application.Ordering.CustomerSessions.Interfaces;
using QROrdering.Domain.Entities.Ordering;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Domain.Enums;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Ordering.CustomerSessions
{
    public class CustomerSessionRepository : ICustomerSessionRepository
    {
        private readonly QROrderingDbContext _context;

        public CustomerSessionRepository(QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RestaurantExistsAsync(Guid restaurantId)
        {
            return await _context.Restaurants
                .AnyAsync(r => r.Id == restaurantId);
        }

        public async Task<bool> RestaurantIsActiveAsync(Guid restaurantId)
        {
            return await _context.Restaurants
                .AnyAsync(r =>
                    r.Id == restaurantId &&
                    r.IsActive);
        }

        public async Task<RestaurantTable?> GetTableForScanAsync(
            Guid restaurantId,
            Guid tableId)
        {
            return await _context.RestaurantTables
                .FirstOrDefaultAsync(t =>
                    t.RestaurantId == restaurantId &&
                    t.Id == tableId);
        }

        public async Task<bool> HasActiveSessionAsync(
            Guid restaurantId,
            Guid tableId)
        {
            return await _context.CustomerSessions
                .AnyAsync(s =>
                    s.RestaurantId == restaurantId &&
                    s.TableId == tableId &&
                    s.Status == CustomerSessionStatus.Active);
        }

        public async Task<int> CountByRestaurantAsync(Guid restaurantId)
        {
            return await _context.CustomerSessions
                .CountAsync(s => s.RestaurantId == restaurantId);
        }

        public async Task<List<CustomerSessionListResponse>> GetPagedByRestaurantAsync(
            Guid restaurantId,
            PagedRequest request)
        {
            return await _context.CustomerSessions
                .AsNoTracking()
                .Where(s => s.RestaurantId == restaurantId)
                .OrderBy(s => s.Status == CustomerSessionStatus.Active ? 0 : 1)
                .ThenByDescending(s => s.StartedAt)
                .ThenBy(s => s.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new CustomerSessionListResponse
                {
                    Id = s.Id,
                    TableId = s.TableId,
                    TableNumber = s.RestaurantTable.TableNumber,
                    Status = s.Status,
                    StartedAt = s.StartedAt,
                    EndedAt = s.EndedAt
                })
                .ToListAsync();
        }

        public async Task<CustomerSessionDetailResponse?> GetByIdInRestaurantAsync(
            Guid restaurantId,
            Guid sessionId)
        {
            return await _context.CustomerSessions
                .AsNoTracking()
                .Where(s =>
                    s.RestaurantId == restaurantId &&
                    s.Id == sessionId)
                .Select(s => new CustomerSessionDetailResponse
                {
                    Id = s.Id,
                    RestaurantId = s.RestaurantId,
                    TableId = s.TableId,
                    TableNumber = s.RestaurantTable.TableNumber,
                    Status = s.Status,
                    StartedAt = s.StartedAt,
                    EndedAt = s.EndedAt,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerSessionDetailResponse?> GetByTokenHashAsync(
            string tokenHash)
        {
            return await _context.CustomerSessions
                .AsNoTracking()
                .Where(s =>
                    s.SessionTokenHash == tokenHash &&
                    s.Status == CustomerSessionStatus.Active)
                .Select(s => new CustomerSessionDetailResponse
                {
                    Id = s.Id,
                    RestaurantId = s.RestaurantId,
                    TableId = s.TableId,
                    TableNumber = s.RestaurantTable.TableNumber,
                    Status = s.Status,
                    StartedAt = s.StartedAt,
                    EndedAt = s.EndedAt,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public Task AddAsync(CustomerSession session)
        {
            _context.CustomerSessions.Add(session);
            return Task.CompletedTask;
        }

        public async Task<CustomerSession?> GetByIdForUpdateAsync(
            Guid restaurantId,
            Guid sessionId)
        {
            return await _context.CustomerSessions
                .FromSqlInterpolated($@"
            SELECT *
            FROM [CustomerSessions] WITH (UPDLOCK, HOLDLOCK)
            WHERE [RestaurantId] = {restaurantId}
              AND [Id] = {sessionId}")
                .AsTracking()
                .FirstOrDefaultAsync();
        }
    }
}
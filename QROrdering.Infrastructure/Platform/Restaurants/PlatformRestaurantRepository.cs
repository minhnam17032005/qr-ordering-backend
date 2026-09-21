using Microsoft.EntityFrameworkCore;

using QROrdering.Application.Platform.Restaurants.Interfaces;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Platform.Restaurants
{
    public class PlatformRestaurantRepository: IPlatformRestaurantRepository
    {
        private readonly QROrderingDbContext _context;

        public PlatformRestaurantRepository(
            QROrderingDbContext context)
        {
            _context = context;
        }


        public async Task<int> CountAsync()
        {
            return await _context.Restaurants
                .CountAsync();
        }


        public async Task<List<Restaurant>> GetPagedAsync(
            int page,
            int pageSize)
        {
            return await _context.Restaurants
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public async Task<Restaurant?> GetByIdAsync(
            Guid restaurantId)
        {
            return await _context.Restaurants
                .FirstOrDefaultAsync(
                    x => x.Id == restaurantId);
        }


        public async Task AddAsync(
            Restaurant restaurant)
        {
            await _context.Restaurants.AddAsync(
                restaurant);
        }
    }
}
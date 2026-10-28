using Microsoft.EntityFrameworkCore;
using QROrdering.Application.Authorization.Interfaces;
using QROrdering.Domain.Entities.Authorization;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Authorization
{
    public class RoleRepository
        : IRoleRepository
    {
        private readonly QROrderingDbContext _context;

        public RoleRepository(
            QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            Role role)
        {
            await _context.Roles
                .AddAsync(role);
        }

        public async Task<List<Role>> GetByIdsInRestaurantAsync(
        Guid restaurantId,
        List<Guid> roleIds)
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(role =>
                    role.RestaurantId == restaurantId &&
                    roleIds.Contains(role.Id))
                .ToListAsync();
        }

        public async Task<List<Role>> GetActiveByRestaurantIdAsync(
            Guid restaurantId)
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(role =>
                    role.RestaurantId == restaurantId &&
                    role.IsActive)
                .OrderBy(role => role.Name)
                .ThenBy(role => role.Id)
                .ToListAsync();
        }
    }
}
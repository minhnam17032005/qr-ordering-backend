using Microsoft.EntityFrameworkCore;
using QROrdering.Application.Platform.Users.Interfaces;
using QROrdering.Domain.Entities.Identity;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Platform.Users
{
    public class PlatformUserRepository : IPlatformUserRepository
    {
        private readonly QROrderingDbContext _context;

        public PlatformUserRepository(
            QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountAsync()
        {
            return await _context.Users
                .CountAsync();
        }

        public async Task<List<User>> GetPagedAsync(
            int page,
            int pageSize)
        {
            return await _context.Users
                .AsNoTracking()

                .Include(x => x.RestaurantMembers)
                    .ThenInclude(x => x.Restaurant)

                .Include(x => x.RestaurantMembers)
                    .ThenInclude(x => x.MemberRoles)
                        .ThenInclude(x => x.Role)

                .OrderByDescending(x => x.CreatedAt)

                .Skip((page - 1) * pageSize)
                .Take(pageSize)

                .ToListAsync();
        }

        public async Task<User?>
            GetByIdWithRestaurantsAsync(
                Guid userId)
        {
            return await _context.Users
                .AsNoTracking()

                .Include(x => x.RestaurantMembers)
                    .ThenInclude(x => x.Restaurant)

                .Include(x => x.RestaurantMembers)
                    .ThenInclude(x => x.MemberRoles)
                        .ThenInclude(x => x.Role)

                .FirstOrDefaultAsync(
                    x => x.Id == userId);
        }

        public async Task<bool> ExistsByUsernameAsync( string username)
        {
            return await _context.Users
                .AnyAsync(x =>
                    x.Username == username);
        }

        public async Task<bool> ExistsByEmailAsync(
            string email)
        {
            return await _context.Users
                .AnyAsync(x =>
                    x.Email == email);
        }

        public async Task<bool> ExistsByUsernameExceptAsync(
            string username,
            Guid userId)
        {
            return await _context.Users
                .AnyAsync(x =>
                    x.Username == username &&
                    x.Id != userId);
        }

        public async Task<bool> ExistsByEmailExceptAsync(
            string email,
            Guid userId)
        {
            return await _context.Users
                .AnyAsync(x =>
                    x.Email == email &&
                    x.Id != userId);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User?> GetByIdForUpdateAsync(
        Guid userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(
                    x => x.Id == userId);
        }
    }
}

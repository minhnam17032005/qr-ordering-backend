using Microsoft.EntityFrameworkCore;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Domain.Entities.Identity;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Authentication
{
    public class UserRepository : IUserRepository
    {
        private readonly QROrderingDbContext _context;

        public UserRepository(QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _context.Users
                .AnyAsync(x => x.Username == username);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users
                .AnyAsync(x => x.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByIdentifierAsync(
        string identifier)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Email == identifier ||
                    x.Username == identifier ||
                    x.PhoneNumber == identifier);
        }

        public async Task<bool> ExistsByUsernameOrEmailOrPhoneAsync(
        string username,
        string email,
        string? phoneNumber)
        {
            return await _context.Users.AnyAsync(x =>
                x.Username == username ||
                x.Email == email ||
                (phoneNumber != null &&
                 x.PhoneNumber == phoneNumber));
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<User?> GetByIdWithRestaurantMembershipsAsync(Guid userId)
        {
            return await _context.Users
                .Include(x => x.RestaurantMembers)
                    .ThenInclude(x => x.Restaurant)
                .Include(x => x.RestaurantMembers)
                    .ThenInclude(x => x.MemberRoles)
                        .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            email = email.Trim().ToLowerInvariant();

            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
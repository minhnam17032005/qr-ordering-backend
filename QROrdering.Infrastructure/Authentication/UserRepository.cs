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

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> ExistsByUsernameOrEmailAsync(
        string username,
        string email)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Username == username ||
                    x.Email == email);
        }
    }
}
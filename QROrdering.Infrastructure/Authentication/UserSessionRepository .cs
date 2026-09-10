using Microsoft.EntityFrameworkCore;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Authentication
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly QROrderingDbContext _context;

        public UserSessionRepository(
            QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserSession session)
        {
            await _context.UserSessions.AddAsync(session);
        }

        public async Task<UserSession?> GetByRefreshTokenHashWithUserAsync(
        string refreshTokenHash)
        {
            return await _context.UserSessions
                .Include(x => x.User)
                .FirstOrDefaultAsync(
                    x => x.RefreshTokenHash == refreshTokenHash);
        }

        public async Task<UserSession?> GetBySessionIdAsync(Guid sessionId)
        {
            return await _context.UserSessions
                .FirstOrDefaultAsync(x => x.Id == sessionId);
        }

        public async Task<UserSession?> GetSessionWithUserAsync(Guid sessionId)
        {
            return await _context.UserSessions
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == sessionId);
        }

        public async Task<List<UserSession>> GetActiveByUserIdAsync(Guid userId)
        {
            return await _context.UserSessions
                .Where(x =>
                    x.UserId == userId &&
                    x.RevokedAt == null)
                .ToListAsync();
        }

        public async Task<List<UserSession>> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserSessions
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId &&
                    x.RevokedAt == null)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<UserSession?> GetByIdAsync(Guid sessionId)
        {
            return await _context.UserSessions
                .FirstOrDefaultAsync(x => x.Id == sessionId);
        }

        public async Task<int> CountByUserIdAsync(Guid userId)
        {
            return await _context.UserSessions
                .CountAsync(x => x.UserId == userId);
        }

        public async Task<List<UserSession>> GetPagedByUserIdAsync(
        Guid userId,
        int page,
        int pageSize)
        {
            return await _context.UserSessions
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}

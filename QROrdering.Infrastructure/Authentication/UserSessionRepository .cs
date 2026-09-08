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
    }
}

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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QROrdering.Application.Platform.Authentication.Interfaces;
using QROrdering.Domain.Entities.Platform;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Platform.Authentication
{
    public class PlatformAdminSessionRepository: IPlatformAdminSessionRepository
    {
        private readonly QROrderingDbContext _context;

        public PlatformAdminSessionRepository(
            QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            PlatformAdminSession session)
        {
            await _context.PlatformAdminSessions
                .AddAsync(session);
        }

        public async Task<PlatformAdminSession?>
        GetSessionWithPlatformAdminAsync(Guid sessionId)
        {
            return await _context.PlatformAdminSessions
                .Include(x => x.PlatformAdmin)
                .FirstOrDefaultAsync(x => x.Id == sessionId);
        }

        public async Task<PlatformAdminSession?>GetByRefreshTokenHashWithPlatformAdminAsync(
        string refreshTokenHash)
        {
            return await _context.PlatformAdminSessions
                .Include(x => x.PlatformAdmin)
                .FirstOrDefaultAsync(
                    x => x.RefreshTokenHash == refreshTokenHash);
        }

        public async Task<PlatformAdminSession?>GetBySessionIdAsync(Guid sessionId)
        {
            return await _context.PlatformAdminSessions
                .FirstOrDefaultAsync(
                    x => x.Id == sessionId);
        }

        public async Task<List<PlatformAdminSession>>GetActiveByPlatformAdminIdAsync(Guid platformAdminId)
        {
            return await _context.PlatformAdminSessions
                .Where(x =>
                    x.PlatformAdminId == platformAdminId &&
                    x.RevokedAt == null &&
                    x.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<int> CountByPlatformAdminIdAsync(
        Guid platformAdminId)
        {
            return await _context.PlatformAdminSessions
                .CountAsync(x =>
                    x.PlatformAdminId == platformAdminId &&
                    x.RevokedAt == null &&
                    x.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<List<PlatformAdminSession>>GetPagedByPlatformAdminIdAsync(
        Guid platformAdminId,
        int page,
        int pageSize)
        {
            return await _context.PlatformAdminSessions
                .Where(x =>
                    x.PlatformAdminId == platformAdminId &&
                    x.RevokedAt == null &&
                    x.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(x => x.LastActivityAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<PlatformAdminSession?> GetByIdAsync( Guid sessionId)
        {
            return await _context.PlatformAdminSessions
                .FirstOrDefaultAsync(
                    x => x.Id == sessionId);
        }
    }
}

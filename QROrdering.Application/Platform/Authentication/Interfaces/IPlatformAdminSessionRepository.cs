using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QROrdering.Domain.Entities.Platform;

namespace QROrdering.Application.Platform.Authentication.Interfaces
{
    public interface IPlatformAdminSessionRepository
    {
        Task AddAsync(PlatformAdminSession session);
        Task<PlatformAdminSession?> GetSessionWithPlatformAdminAsync(
        Guid sessionId);
        Task<PlatformAdminSession?>GetByRefreshTokenHashWithPlatformAdminAsync(
        string refreshTokenHash);

        Task<PlatformAdminSession?>GetBySessionIdAsync(Guid sessionId);

        Task<List<PlatformAdminSession>>GetActiveByPlatformAdminIdAsync(Guid platformAdminId);

        Task<int> CountByPlatformAdminIdAsync(Guid platformAdminId);

        Task<List<PlatformAdminSession>>GetPagedByPlatformAdminIdAsync(
                Guid platformAdminId,
                int page,
                int pageSize);

        Task<PlatformAdminSession?> GetByIdAsync(Guid sessionId);
    }
}

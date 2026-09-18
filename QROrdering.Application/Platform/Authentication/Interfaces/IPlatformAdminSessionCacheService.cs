using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QROrdering.Application.Platform.Authentication.DTOs.Responses;
using QROrdering.Domain.Entities.Platform;

namespace QROrdering.Application.Platform.Authentication.Interfaces
{
    public interface IPlatformAdminSessionCacheService
    {
        Task<CachedPlatformAdminSession?> GetAsync(
            Guid sessionId);

        Task SetAsync(
            PlatformAdminSession session,
            bool isPlatformAdminActive);

        Task RemoveAsync(
            Guid sessionId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QROrdering.Application.Common.Redis;

namespace QROrdering.Application.Common.Interfaces
{
    public interface ISessionCacheService
    {
        Task<CachedSession?> GetAsync(Guid sessionId);

        Task SetAsync(UserSession userSession, bool isUserActive);

        Task RemoveAsync(Guid sessionId);
    }
}

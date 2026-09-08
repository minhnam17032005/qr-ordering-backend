using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QROrdering.Application.Authentication.Interfaces
{
    public interface IJwtBlacklistService
    {
        Task BlacklistTokenAsync(
            string? jti,
            string? expClaim);

        Task<bool> IsBlacklistedAsync(string jti);
    }
}

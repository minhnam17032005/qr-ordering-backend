using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Infrastructure.Redis;

namespace QROrdering.Infrastructure.Authentication
{
    public class JwtBlacklistService : IJwtBlacklistService
    {
        private readonly RedisService _redisService;

        public JwtBlacklistService(RedisService redisService)
        {
            _redisService = redisService;
        }

        // Đưa access token vào blacklist
        public async Task BlacklistTokenAsync(
            string? jti,
            string? expClaim)
        {
            if (string.IsNullOrWhiteSpace(jti))
                return;

            if (!long.TryParse(expClaim, out var expUnix))
                return;

            var expiredAt = DateTimeOffset
                .FromUnixTimeSeconds(expUnix)
                .UtcDateTime;

            var ttl = expiredAt - DateTime.UtcNow;

            if (ttl <= TimeSpan.Zero)
                return;

            await _redisService.SetAsync(
                RedisKeys.BlacklistToken(jti),
                "revoked",
                ttl);
        }

        // Kiểm tra access token có bị blacklist không
        public async Task<bool> IsBlacklistedAsync(string jti)
        {
            return await _redisService.ExistsAsync(
                RedisKeys.BlacklistToken(jti));
        }
    }
}

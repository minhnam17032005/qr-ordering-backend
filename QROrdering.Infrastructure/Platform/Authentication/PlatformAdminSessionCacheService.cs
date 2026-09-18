
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Platform.Authentication.DTOs.Responses;
using QROrdering.Application.Platform.Authentication.Interfaces;
using QROrdering.Domain.Entities.Platform;
using QROrdering.Infrastructure.Configurations;
using QROrdering.Infrastructure.Redis;
using QROrdering.Infrastructure.Redis.Models;

namespace QROrdering.Infrastructure.Platform.Authentication
{
    public class PlatformAdminSessionCacheService
        : IPlatformAdminSessionCacheService
    {
        private readonly IRedisService _redisService;
        private readonly IPlatformAdminSessionRepository _platformAdminSessionRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<PlatformAdminSessionCacheService>_logger;
        private readonly CacheSettings _cacheSettings;

        public PlatformAdminSessionCacheService(
            IRedisService redisService,
            IPlatformAdminSessionRepository
                platformAdminSessionRepository,
            IMemoryCache memoryCache,
            ILogger<PlatformAdminSessionCacheService> logger,
            IOptions<CacheSettings> cacheOptions)
        {
            _redisService = redisService;
            _platformAdminSessionRepository = platformAdminSessionRepository;
            _memoryCache = memoryCache;
            _logger = logger;
            _cacheSettings = cacheOptions.Value;
        }

        /// <summary>
        /// Cache Aside Pattern
        /// Memory -> Redis -> Database
        /// </summary>
        public async Task<CachedPlatformAdminSession?>
            GetAsync(Guid sessionId)
        {
            var cacheKey =
                RedisKeys.PlatformAdminSession(sessionId);

            // 1. Memory Cache - L1
            if (_memoryCache.TryGetValue(
                cacheKey,
                out RedisPlatformAdminSession? redisSession))
            {
                Console.WriteLine(
                    $"[PLATFORM SESSION CACHE] MEMORY HIT -> {sessionId}");

                return ToCachedPlatformAdminSession(
                    redisSession!);
            }

            // 2. Redis Cache - L2
            redisSession =
                await _redisService
                    .GetAsync<RedisPlatformAdminSession>(
                        cacheKey);

            if (redisSession != null)
            {
                Console.WriteLine(
                    $"[PLATFORM SESSION CACHE] REDIS HIT -> {sessionId}");

                _memoryCache.Set(
                    cacheKey,
                    redisSession,
                    TimeSpan.FromSeconds(
                        _cacheSettings.Session
                            .MemoryExpirationSeconds));

                return ToCachedPlatformAdminSession(
                    redisSession);
            }

            // 3. Cache MISS
            Console.WriteLine(
                $"[PLATFORM SESSION CACHE] MISS -> {sessionId}");

            // 4. Database
            var platformAdminSession =
                await _platformAdminSessionRepository
                    .GetSessionWithPlatformAdminAsync(
                        sessionId);

            if (platformAdminSession == null)
            {
                _logger.LogWarning(
                    "PlatformAdmin session {SessionId} not found.",
                    sessionId);

                return null;
            }

            // 5. Database -> Cache
            await SetAsync(
                platformAdminSession,
                platformAdminSession.PlatformAdmin.IsActive);

            return ToCachedPlatformAdminSession(
                ToRedisPlatformAdminSession(
                    platformAdminSession,
                    platformAdminSession.PlatformAdmin.IsActive));
        }

        /// <summary>
        /// Ghi PlatformAdmin Session vào Memory + Redis
        /// </summary>
        public async Task SetAsync(
            PlatformAdminSession session,
            bool isPlatformAdminActive)
        {
            var cacheKey =
                RedisKeys.PlatformAdminSession(
                    session.Id);

            var redisSession =
                ToRedisPlatformAdminSession(
                    session,
                    isPlatformAdminActive);

            var sessionTtl =
                session.ExpiresAt - DateTime.UtcNow;

            var configuredTtl =
                TimeSpan.FromMinutes(
                    _cacheSettings.Session
                        .RedisExpirationMinutes);

            var ttl =
                sessionTtl < configuredTtl
                    ? sessionTtl
                    : configuredTtl;

            if (ttl > TimeSpan.Zero)
            {
                await _redisService.SetAsync(
                    cacheKey,
                    redisSession,
                    ttl);
            }

            _memoryCache.Set(
                cacheKey,
                redisSession,
                TimeSpan.FromSeconds(
                    _cacheSettings.Session
                        .MemoryExpirationSeconds));

            Console.WriteLine(
                $"[PLATFORM SESSION CACHE] SET -> {session.Id}");

            _logger.LogInformation(
                "PlatformAdmin Session Cache SET for {SessionId}",
                session.Id);
        }

        /// <summary>
        /// Xóa cache của một PlatformAdmin Session
        /// </summary>
        public async Task RemoveAsync(Guid sessionId)
        {
            var cacheKey =
                RedisKeys.PlatformAdminSession(sessionId);

            _memoryCache.Remove(cacheKey);

            await _redisService.RemoveAsync(cacheKey);

            Console.WriteLine(
                $"[PLATFORM SESSION CACHE] REMOVE -> {sessionId}");

            _logger.LogInformation(
                "PlatformAdmin Session Cache REMOVED for {SessionId}",
                sessionId);
        }

        /// <summary>
        /// PlatformAdminSession Entity -> Redis Model
        /// </summary>
        private static RedisPlatformAdminSession
            ToRedisPlatformAdminSession(
                PlatformAdminSession session,
                bool isPlatformAdminActive)
        {
            return new RedisPlatformAdminSession
            {
                SessionId = session.Id,
                PlatformAdminId =
                    session.PlatformAdminId,
                IsRevoked =
                    session.RevokedAt != null,
                IsPlatformAdminActive =
                    isPlatformAdminActive,
                ExpiresAt =
                    session.ExpiresAt
            };
        }

        /// <summary>
        /// Redis Model -> Application Model
        /// </summary>
        private static CachedPlatformAdminSession
            ToCachedPlatformAdminSession(
                RedisPlatformAdminSession session)
        {
            return new CachedPlatformAdminSession
            {
                SessionId = session.SessionId,
                PlatformAdminId =
                    session.PlatformAdminId,
                IsRevoked =
                    session.IsRevoked,
                IsPlatformAdminActive =
                    session.IsPlatformAdminActive,
                ExpiresAt =
                    session.ExpiresAt
            };
        }
    }
}

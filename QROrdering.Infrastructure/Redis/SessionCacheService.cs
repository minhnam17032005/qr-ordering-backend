using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QROrdering.Application.Authentication.DTOs.Redis;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Infrastructure.Configurations;
using QROrdering.Infrastructure.DTOs;

namespace QROrdering.Infrastructure.Redis
{
    public class SessionCacheService : ISessionCacheService
    {
        private readonly RedisService _redisService;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<SessionCacheService> _logger;
        private readonly CacheSettings _cacheSettings;

        public SessionCacheService(
            RedisService redisService,
            IUserSessionRepository userSessionRepository,
            IMemoryCache memoryCache,
            ILogger<SessionCacheService> logger,
            IOptions<CacheSettings> cacheOptions)
        {
            _redisService = redisService;
            _userSessionRepository = userSessionRepository;
            _memoryCache = memoryCache;
            _logger = logger;
            _cacheSettings = cacheOptions.Value;
        }

        /// <summary>
        /// Cache Aside Pattern
        /// Memory -> Redis -> Database
        /// </summary>
        public async Task<CachedSession?> GetAsync(Guid sessionId)
        {
            var cacheKey = RedisKeys.Session(sessionId);

            // 1. Memory Cache - L1
            if (_memoryCache.TryGetValue(
                cacheKey,
                out RedisUserSession? redisSession))
            {
                Console.WriteLine(
                    $"[SESSION CACHE] MEMORY HIT -> {sessionId}");

                return ToCachedSession(redisSession!);
            }

            // 2. Redis Cache - L2
            redisSession = await _redisService
                .GetAsync<RedisUserSession>(cacheKey);

            if (redisSession != null)
            {
                Console.WriteLine(
                    $"[SESSION CACHE] REDIS HIT -> {sessionId}");

                _memoryCache.Set(
                    cacheKey,
                    redisSession,
                    TimeSpan.FromSeconds(
                        _cacheSettings.Session.MemoryExpirationSeconds));

                return ToCachedSession(redisSession);
            }

            // 3. Cache MISS
            Console.WriteLine(
                $"[SESSION CACHE] MISS -> {sessionId}");

            // 4. Database
            var userSession =
                await _userSessionRepository
                    .GetSessionWithUserAsync(sessionId);

            if (userSession == null)
            {
                _logger.LogWarning(
                    "Session {SessionId} not found.",
                    sessionId);

                return null;
            }

            // 5. Database -> Cache
            await SetAsync(
                userSession,
                userSession.User.IsActive);

            return ToCachedSession(
                ToRedisUserSession(
                    userSession,
                    userSession.User.IsActive));
        }

        /// <summary>
        /// Ghi Session vào Memory + Redis
        /// </summary>
        public async Task SetAsync(
        UserSession userSession,
        bool isUserActive)
        {
            var cacheKey =
                RedisKeys.Session(userSession.Id);

            var redisSession =
                ToRedisUserSession(
                    userSession,
                    isUserActive);

            var sessionTtl =
                userSession.ExpiredAt - DateTime.UtcNow;

            var configuredTtl =
                TimeSpan.FromMinutes(
                    _cacheSettings.Session.RedisExpirationMinutes);

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
                    _cacheSettings.Session.MemoryExpirationSeconds));

            Console.WriteLine(
                $"[SESSION CACHE] SET -> {userSession.Id}");

            _logger.LogInformation(
                "Session Cache SET for {SessionId}",
                userSession.Id);
        }

        /// <summary>
        /// Xóa cache của một Session
        /// </summary>
        public async Task RemoveAsync(Guid sessionId)
        {
            var cacheKey = RedisKeys.Session(sessionId);

            _memoryCache.Remove(cacheKey);

            await _redisService.RemoveAsync(cacheKey);

            Console.WriteLine(
                $"[SESSION CACHE] REMOVE -> {sessionId}");

            _logger.LogInformation(
                "Session Cache REMOVED for {SessionId}",
                sessionId);
        }

        /// <summary>
        /// UserSession Entity -> Redis Model
        /// </summary>
        private static RedisUserSession ToRedisUserSession(
        UserSession session,
        bool isUserActive)
        {
            return new RedisUserSession
            {
                SessionId = session.Id,
                UserId = session.UserId,
                IsRevoked = session.RevokedAt != null,
                IsUserActive = isUserActive,
                ExpiredAt = session.ExpiredAt
            };
        }

        /// <summary>
        /// Redis Model -> Application Model
        /// </summary>
        private static CachedSession ToCachedSession(
        RedisUserSession session)
        {
            return new CachedSession
            {
                SessionId = session.SessionId,
                UserId = session.UserId,
                IsRevoked = session.IsRevoked,
                IsUserActive = session.IsUserActive,
                ExpiredAt = session.ExpiredAt
            };
        }
    }
}
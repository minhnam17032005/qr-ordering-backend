using Microsoft.Extensions.Logging;
using QROrdering.Application.Authentication.DTOs;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Exceptions;
using QROrdering.Domain.Entities.Identity;

namespace QROrdering.Application.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestInfoService _requestInfoService;
        private readonly IHashService _hashService;
        private readonly ICurrentUserService _currentUser;
        private readonly IJwtBlacklistService _jwtBlacklistService;
        private readonly ILogger<AuthService> _logger;
        private readonly ISessionCacheService _sessionCacheService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IJwtService jwtService,
            IUserSessionRepository userSessionRepository,
            IRequestInfoService requestInfoService,
            IUnitOfWork unitOfWork,
            IHashService hashService,
            ICurrentUserService currentUser,
            IJwtBlacklistService jwtBlacklistService,
            ISessionCacheService sessionCacheService,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _userSessionRepository = userSessionRepository;
            _unitOfWork = unitOfWork;
            _requestInfoService = requestInfoService;
            _hashService = hashService;
            _currentUser = currentUser;
            _jwtBlacklistService = jwtBlacklistService;
            _sessionCacheService = sessionCacheService;
            _logger = logger;
        }

        public async Task<RegisterResponse> RegisterAsync(
            RegisterRequest request)
        {
            // 1. Normalize input
            var fullName = request.FullName.Trim();
            var username = request.Username.Trim().ToLowerInvariant();
            var email = request.Email.Trim().ToLowerInvariant();
            var phoneNumber = request.PhoneNumber?.Trim();

            // Check duplicate identity
            var exists =
                await _userRepository.ExistsByUsernameOrEmailOrPhoneAsync(
                    username,
                    email,
                    phoneNumber);

            if (exists)
            {
                throw new ConflictException(
                    "Username, email hoặc số điện thoại đã tồn tại.");
            }

            //Hash password
            var passwordHash = _passwordService.Hash(request.Password);

            var user = new User
            {
                FullName = fullName,
                Username = username,
                Email = email,
                PhoneNumber = phoneNumber,
                PasswordHash = passwordHash,
                IsActive = true
            };

            //Save User
            await _userRepository.AddAsync(user);

            // Commit transaction
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
            "User {UserId} registered successfully",
            user.Id);

            //Return response
            return new RegisterResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<(LoginResponse response, string refreshToken)> LoginAsync(
        LoginRequest request)
        {
            var identifier = request.Identifier.Trim().ToLowerInvariant();

            var user =
                await _userRepository.GetByIdentifierAsync(identifier);

            if (user == null || !_passwordService.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedException(
                    "Identifier hoặc password không hợp lệ.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedException(
                    "Tài khoản đã bị khóa.");
            }

            // Tạo Session Id trước
            var sessionId = Guid.NewGuid();

            // Tạo Access Token + Refresh Token
            var accessToken =
                _jwtService.GenerateAccessToken(
                    user,
                    sessionId);

            var refreshToken =
                _jwtService.GenerateRefreshToken();

            // Tạo phiên đăng nhập
            var session = new UserSession
            {
                Id = sessionId,
                UserId = user.Id,

                // Lưu hash của refresh token
                RefreshTokenHash = _hashService.Hash(refreshToken),

                // Thông tin request
                DeviceName =
                    _requestInfoService.DeviceName,

                IpAddress =
                    _requestInfoService.IpAddress,

                UserAgent =
                    _requestInfoService.UserAgent,

                // Thời điểm truy cập gần nhất
                LastAccessAt = DateTime.UtcNow,

                // Thời gian hết hạn refresh token
                ExpiredAt =
                    _jwtService.GetRefreshTokenExpiration(),

                // Login mới => chưa revoke
                RevokedAt = null
            };
            await _userSessionRepository.AddAsync(session);
            await _unitOfWork.SaveChangesAsync();

            // Cache session
            await _sessionCacheService.SetAsync(
             session,
             user.IsActive);

            _logger.LogInformation(
            "User {UserId} logged in successfully. Session {SessionId}",
            user.Id,
            session.Id);

            var response = new LoginResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                AccessToken = accessToken
            };

            return (response, refreshToken);
        }

        public async Task<(RefreshResponse response, string refreshToken)> RefreshTokenAsync(
        string refreshToken)
        {
            // Hash refresh token để tìm session
            var refreshTokenHash = _hashService.Hash(refreshToken);

            // Tìm session kèm User
            var session =
                await _userSessionRepository
                    .GetByRefreshTokenHashWithUserAsync(refreshTokenHash);

            if (session == null)
            {
                throw new UnauthorizedException("Phiên đăng nhập không hợp lệ.");
            }

            // Kiểm tra session đã bị thu hồi
            if (session.RevokedAt != null)
            {
                throw new UnauthorizedException("Phiên đăng nhập đã bị thu hồi.");
            }

            // Kiểm tra refresh token hết hạn
            if (session.ExpiredAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedException("Phiên đăng nhập đã hết hạn.");
            }

            // Kiểm tra tài khoản
            if (!session.User.IsActive)
            {
                throw new UnauthorizedException("Tài khoản đã bị khóa.");
            }

            // Tạo Access Token mới
            var accessToken =
                _jwtService.GenerateAccessToken(
                    session.User,
                    session.Id);

            // Rotate Refresh Token
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            session.RefreshTokenHash = _hashService.Hash(newRefreshToken);

            // Cập nhật thời gian truy cập
            session.LastAccessAt = DateTime.UtcNow;

            // Gia hạn refresh token
            session.ExpiredAt = _jwtService.GetRefreshTokenExpiration();
            await _unitOfWork.SaveChangesAsync();

            // Update Session Cache
            await _sessionCacheService.SetAsync(
                session,
                session.User.IsActive);

            _logger.LogInformation(
            "User {UserId} refreshed access token. Session {SessionId}",
            session.UserId,
            session.Id);

            return (
                new RefreshResponse
                {
                    AccessToken = accessToken
                },
                newRefreshToken
            );
        }


        public async Task<UserProfileResponse> GetProfileAsync()
        {
            var userId = _currentUser.UserId;

            var user = await _userRepository
                .GetByIdWithRestaurantMembershipsAsync(userId);

            if (user == null)
            {
                throw new NotFoundException(
                    "Người dùng không tồn tại.");
            }

            return new UserProfileResponse
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl,

                Restaurants = user.RestaurantMembers
                    .Select(member => new RestaurantMembershipResponse
                    {
                        RestaurantId = member.RestaurantId,
                        RestaurantName = member.Restaurant.Name,

                        Roles = member.MemberRoles
                            .Select(memberRole => new MemberRoleResponse
                            {
                                RoleId = memberRole.RoleId,
                                RoleName = memberRole.Role.Name
                            })
                            .ToList()
                    })
                    .ToList(),

                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt ?? user.CreatedAt
            };
        }

        public async Task LogoutAsync()
        {
            var sessionId = _currentUser.SessionId;

            var session = await _userSessionRepository
                .GetBySessionIdAsync(sessionId);

            if (session == null)
            {
                throw new NotFoundException(
                    "Phiên đăng nhập không tồn tại.");
            }

            await _jwtBlacklistService.BlacklistTokenAsync(
                _currentUser.Jti,
                _currentUser.ExpiredAtString);

            session.RevokedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            await _sessionCacheService.RemoveAsync(sessionId);

            _logger.LogInformation(
                "User {UserId} logged out. Session {SessionId}",
                _currentUser.UserId,
                sessionId);
        }

        public async Task LogoutAllSessionsAsync()
        {
            var userId = _currentUser.UserId;

            var sessions = await _userSessionRepository
                .GetActiveByUserIdAsync(userId);

            // REVOKE ALL SESSIONS
            var revokedAt = DateTime.UtcNow;

            foreach (var session in sessions)
            {
                session.RevokedAt = revokedAt;
            }

            // BLACKLIST CURRENT ACCESS TOKEN
            await _jwtBlacklistService.BlacklistTokenAsync(
                _currentUser.Jti,
                _currentUser.ExpiredAtString);

            await _unitOfWork.SaveChangesAsync();

            // INVALIDATE SESSION CACHE
            foreach (var session in sessions)
            {
                await _sessionCacheService.RemoveAsync(session.Id);
            }

            _logger.LogInformation(
                "User {UserId} logged out from all sessions.",
                userId);
        }

        public async Task LogoutOtherSessionsAsync()
        {
            var userId = _currentUser.UserId;
            var currentSessionId = _currentUser.SessionId;

            var sessions = await _userSessionRepository
                .GetActiveByUserIdAsync(userId);

            // REVOKE ALL OTHER SESSIONS
            var revokedAt = DateTime.UtcNow;

            var otherSessions = sessions
                .Where(session => session.Id != currentSessionId)
                .ToList();

            foreach (var session in otherSessions)
            {
                session.RevokedAt = revokedAt;
            }

            await _unitOfWork.SaveChangesAsync();

            // INVALIDATE OTHER SESSION CACHES
            foreach (var session in otherSessions)
            {
                await _sessionCacheService.RemoveAsync(session.Id);
            }

            _logger.LogInformation(
                "User {UserId} logged out from all other sessions. Current session {SessionId} kept active.",
                userId,
                currentSessionId);
        }
        public async Task<PagedResponse<UserSessionResponse>> GetSessionsAsync(
        PagedRequest request)
        {
            var userId = _currentUser.UserId;
            var currentSessionId = _currentUser.SessionId;

            var totalItems = await _userSessionRepository
                .CountByUserIdAsync(userId);

            var sessions = await _userSessionRepository
                .GetPagedByUserIdAsync(
                    userId,
                    request.Page,
                    request.PageSize);

            var items = sessions
                .Select(session => new UserSessionResponse
                {
                    SessionId = session.Id,
                    DeviceName = session.DeviceName,
                    IpAddress = session.IpAddress,
                    UserAgent = session.UserAgent,
                    CreatedAt = session.CreatedAt,
                    LastAccessAt = session.LastAccessAt,
                    ExpiredAt = session.ExpiredAt,
                    IsCurrent = session.Id == currentSessionId
                })
                .ToList();

            return new PagedResponse<UserSessionResponse>
            {
                Items = items,
                Pagination = new PaginationMeta
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling(
                        totalItems / (double)request.PageSize)
                }
            };
        }

        public async Task DeleteSessionAsync(Guid sessionId)
        {
            var userId = _currentUser.UserId;

            var session = await _userSessionRepository
                .GetByIdAsync(sessionId);

            if (session == null)
            {
                throw new NotFoundException(
                    "Session không tồn tại.");
            }

            // OWNERSHIP CHECK
            if (session.UserId != userId)
            {
                throw new NotFoundException(
                    "Session không tồn tại.");
            }

            // REVOKE SESSION
            session.RevokedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            // INVALIDATE SESSION CACHE
            await _sessionCacheService.RemoveAsync(session.Id);
        }
    }
}
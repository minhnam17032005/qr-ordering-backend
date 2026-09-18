using Microsoft.Extensions.Logging;
using QROrdering.Application.Common.DTOs;
using QROrdering.Application.Common.Enums;
using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Exceptions;
using QROrdering.Application.Platform.Authentication.DTOs.Requests;
using QROrdering.Application.Platform.Authentication.DTOs.Responses;
using QROrdering.Application.Platform.Authentication.Interfaces;

using QROrdering.Domain.Entities.Platform;

namespace QROrdering.Application.Platform.Authentication
{
    public class PlatformAdminAuthService : IPlatformAdminAuthService
    {
        private readonly IPlatformAdminRepository _platformAdminRepository;
        private readonly IPlatformAdminSessionRepository _platformAdminSessionRepository;

        private readonly IPasswordService _passwordService;
        private readonly ICurrentUserService _currentUser;
        private readonly IJwtService _jwtService;
        private readonly IHashService _hashService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtBlacklistService _jwtBlacklistService;
        private readonly IRequestInfoService _requestInfoService;
        private readonly IPlatformAdminSessionCacheService _platformAdminSessionCacheService;
        private readonly IOtpService _otpService;
        private readonly ILogger<PlatformAdminAuthService> _logger;

        public PlatformAdminAuthService(
            IPlatformAdminRepository platformAdminRepository,
            IPlatformAdminSessionRepository platformAdminSessionRepository,
            IPasswordService passwordService,
            IJwtService jwtService,
            IHashService hashService,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            IJwtBlacklistService jwtBlacklistService,
            IRequestInfoService requestInfoService,
            IPlatformAdminSessionCacheService platformAdminSessionCacheService,
            IOtpService otpService,
            ILogger<PlatformAdminAuthService> logger)
        {
            _platformAdminRepository = platformAdminRepository;
            _platformAdminSessionRepository =
                platformAdminSessionRepository;

            _passwordService = passwordService;
            _jwtService = jwtService;
            _hashService = hashService;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
            _jwtBlacklistService = jwtBlacklistService;
            _requestInfoService = requestInfoService;
            _platformAdminSessionCacheService =platformAdminSessionCacheService;
            _logger = logger;
            _otpService = otpService;
        }

        public async Task<(PlatformAdminLoginResponse response, string platformRefreshToken)> LoginAsync(
            PlatformAdminLoginRequest request)
        {
            // 1. Normalize identifier
            var identifier =
                request.Identifier.Trim().ToLowerInvariant();

            // 2. Get PlatformAdmin
            var admin =
                await _platformAdminRepository.GetByIdentifierAsync(identifier);

            // 3. Verify credentials
            if (admin == null ||!_passwordService.Verify( request.Password,admin.PasswordHash))
            {
                throw new UnauthorizedException(
                    "Identifier hoặc password không hợp lệ.");
            }

            // 4. Check account status
            if (!admin.IsActive)
            {
                throw new UnauthorizedException(
                    "Tài khoản đã bị khóa.");
            }

            // 5. Create Session Id
            var sessionId = Guid.NewGuid();

            // 6. Generate Access Token + Refresh Token
            var accessToken =
                _jwtService.GeneratePlatformAdminAccessToken(
                    admin,
                    sessionId);

            var refreshToken =
                _jwtService.GenerateRefreshToken();

            // 7. Create PlatformAdmin Session
            var session = new PlatformAdminSession
            {
                Id = sessionId,

                PlatformAdminId = admin.Id,

                RefreshTokenHash =
                    _hashService.Hash(refreshToken),

                DeviceName =
                    _requestInfoService.DeviceName,

                IpAddress =
                    _requestInfoService.IpAddress,

                UserAgent =
                    _requestInfoService.UserAgent,

                LastActivityAt =
                    DateTime.UtcNow,

                ExpiresAt =
                    _jwtService.GetRefreshTokenExpiration(),

                RevokedAt = null
            };

            // 8. Add session
            await _platformAdminSessionRepository
                .AddAsync(session);

            // 9. Save database changes
            await _unitOfWork.SaveChangesAsync();

            // 10. Cache PlatformAdmin session
            await _platformAdminSessionCacheService.SetAsync(
                session,
                admin.IsActive);

            _logger.LogInformation(
                "PlatformAdmin {PlatformAdminId} logged in successfully. Session {SessionId}",
                admin.Id,
                session.Id);

            // 11. Build response
            var response = new PlatformAdminLoginResponse
            {
                UserId = admin.Id,
                FullName = admin.FullName,
                Username = admin.Username,
                AccessToken = accessToken
            };

            return (response, refreshToken);
        }

        public async Task<(PlatformAdminRefreshResponse response,string refreshToken)> RefreshTokenAsync(string refreshToken)
        {
            // Hash refresh token để tìm session
            var refreshTokenHash =
                _hashService.Hash(refreshToken);

            // Tìm session kèm PlatformAdmin
            var session =
                await _platformAdminSessionRepository
                    .GetByRefreshTokenHashWithPlatformAdminAsync(
                        refreshTokenHash);

            if (session == null)
            {
                throw new UnauthorizedException(
                    "Phiên đăng nhập không hợp lệ.");
            }

            // Kiểm tra session đã bị thu hồi
            if (session.RevokedAt != null)
            {
                throw new UnauthorizedException(
                    "Phiên đăng nhập đã bị thu hồi.");
            }

            // Kiểm tra refresh token hết hạn
            if (session.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedException(
                    "Phiên đăng nhập đã hết hạn.");
            }

            // Kiểm tra Platform Admin
            if (!session.PlatformAdmin.IsActive)
            {
                throw new UnauthorizedException(
                    "Tài khoản Platform Admin đã bị khóa.");
            }

            // Tạo Access Token mới
            var accessToken =
                _jwtService.GeneratePlatformAdminAccessToken(
                    session.PlatformAdmin,
                    session.Id);

            // Rotate Refresh Token
            var newRefreshToken =
                _jwtService.GenerateRefreshToken();

            session.RefreshTokenHash =
                _hashService.Hash(newRefreshToken);

            // Cập nhật thời gian truy cập
            session.LastActivityAt =
                DateTime.UtcNow;

            // Gia hạn refresh token
            session.ExpiresAt =
                _jwtService.GetRefreshTokenExpiration();

            await _unitOfWork.SaveChangesAsync();

            // Update Platform Admin Session Cache
            await _platformAdminSessionCacheService.SetAsync(
                session,
                session.PlatformAdmin.IsActive);

            _logger.LogInformation(
                "PlatformAdmin {PlatformAdminId} refreshed access token. Session {SessionId}",
                session.PlatformAdminId,
                session.Id);

            return (
                new PlatformAdminRefreshResponse
                {
                    AccessToken = accessToken
                },
                newRefreshToken
            );
        }

        public async Task<PlatformAdminProfileResponse> GetProfileAsync()
        {
            var platformAdminId = _currentUser.UserId;

            var admin =
                await _platformAdminRepository
                    .GetByIdAsync(platformAdminId);

            if (admin == null)
            {
                throw new NotFoundException(
                    "Platform Admin không tồn tại.");
            }

            return new PlatformAdminProfileResponse
            {
                Id = admin.Id,
                Username = admin.Username,
                FullName = admin.FullName,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
                AvatarUrl = admin.AvatarUrl,
                CreatedAt = admin.CreatedAt,
                UpdatedAt = admin.UpdatedAt ?? admin.CreatedAt
            };
        }

        public async Task LogoutAsync()
        {
            var sessionId = _currentUser.SessionId;

            var session =
                await _platformAdminSessionRepository
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

            await _platformAdminSessionCacheService
                .RemoveAsync(sessionId);

            _logger.LogInformation(
                "PlatformAdmin {PlatformAdminId} logged out. Session {SessionId}",
                _currentUser.UserId,
                sessionId);
        }

        public async Task LogoutAllSessionsAsync()
        {
            var platformAdminId = _currentUser.UserId;

            var sessions =
                await _platformAdminSessionRepository.GetActiveByPlatformAdminIdAsync(platformAdminId);

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
                await _platformAdminSessionCacheService
                    .RemoveAsync(session.Id);
            }

            _logger.LogInformation(
                "PlatformAdmin {PlatformAdminId} logged out from all sessions.",
                platformAdminId);
        }

        public async Task<PagedResponse<PlatformAdminSessionResponse>>GetSessionsAsync(PagedRequest request)
        {
            var platformAdminId = _currentUser.UserId;
            var currentSessionId = _currentUser.SessionId;

            var totalItems =
                await _platformAdminSessionRepository
                    .CountByPlatformAdminIdAsync(
                        platformAdminId);

            var sessions =
                await _platformAdminSessionRepository
                    .GetPagedByPlatformAdminIdAsync(
                        platformAdminId,
                        request.Page,
                        request.PageSize);

            var items = sessions
                .Select(session => new PlatformAdminSessionResponse
                {
                    SessionId = session.Id,
                    DeviceName = session.DeviceName,
                    IpAddress = session.IpAddress,
                    UserAgent = session.UserAgent,
                    CreatedAt = session.CreatedAt,
                    LastActivityAt = session.LastActivityAt,
                    ExpiresAt = session.ExpiresAt,
                    IsCurrent = session.Id == currentSessionId
                })
                .ToList();

            return new PagedResponse<PlatformAdminSessionResponse>
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
            var platformAdminId = _currentUser.UserId;

            var session =
                await _platformAdminSessionRepository
                    .GetByIdAsync(sessionId);

            if (session == null)
            {
                throw new NotFoundException(
                    "Session không tồn tại.");
            }

            // OWNERSHIP CHECK
            if (session.PlatformAdminId != platformAdminId)
            {
                throw new NotFoundException(
                    "Session không tồn tại.");
            }

            // REVOKE SESSION
            session.RevokedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            // INVALIDATE SESSION CACHE
            await _platformAdminSessionCacheService
                .RemoveAsync(session.Id);
        }

        public async Task SendChangePasswordOtpAsync()
        {
            var platformAdmin =
                await _platformAdminRepository.GetByIdAsync(
                    _currentUser.UserId);

            if (platformAdmin == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy Platform Admin.");
            }

            var account = new OtpAccount
            {
                Id = platformAdmin.Id,
                Email = platformAdmin.Email,
                AccountType = OtpAccountType.PlatformAdmin
            };

            await _otpService.SendChangePasswordOtpAsync(
                account);
        }

        public async Task<PlatformAdminVerifyOtpResponse>VerifyChangePasswordOtpAsync(
        PlatformAdminVerifyChangePasswordOtpRequest request)
        {
            var platformAdmin =
                await _platformAdminRepository.GetByIdAsync(
                    _currentUser.UserId);

            if (platformAdmin == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy Platform Admin.");
            }

            var account = new OtpAccount
            {
                Id = platformAdmin.Id,
                Email = platformAdmin.Email,
                AccountType = OtpAccountType.PlatformAdmin
            };

            var result =
                await _otpService.VerifyChangePasswordOtpAsync(
                    account,
                    request.Otp);

            return new PlatformAdminVerifyOtpResponse
            {
                VerificationToken = result.VerificationToken,
                ExpiredAt = result.ExpiredAt
            };
        }

        public async Task ChangePasswordAsync(PlatformAdminChangePasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                throw new BadRequestException(
                    "Mật khẩu xác nhận không trùng với mật khẩu mới.");
            }

            var platformAdmin =
                await _platformAdminRepository.GetByIdAsync(
                    _currentUser.UserId);

            if (platformAdmin == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy Platform Admin.");
            }

            if (!_passwordService.Verify(
                    request.CurrentPassword,
                    platformAdmin.PasswordHash))
            {
                throw new BadRequestException(
                    "Mật khẩu hiện tại không đúng.");
            }

            if (_passwordService.Verify(
                    request.NewPassword,
                    platformAdmin.PasswordHash))
            {
                throw new BadRequestException(
                    "Mật khẩu mới phải khác mật khẩu hiện tại.");
            }

            var account = new OtpAccount
            {
                Id = platformAdmin.Id,
                Email = platformAdmin.Email,
                AccountType = OtpAccountType.PlatformAdmin
            };

            await _otpService.ValidateChangePasswordVerificationAsync(
                account,
                request.VerificationToken);

            var sessions =
                await _platformAdminSessionRepository
                    .GetActiveByPlatformAdminIdAsync(
                        platformAdmin.Id);

            var revokedAt = DateTime.UtcNow;

            platformAdmin.PasswordHash =
                _passwordService.Hash(
                    request.NewPassword);

            platformAdmin.UpdatedAt = revokedAt;

            foreach (var session in sessions)
            {
                session.RevokedAt = revokedAt;
            }

            await _unitOfWork.SaveChangesAsync();

            foreach (var session in sessions)
            {
                await _platformAdminSessionCacheService
                    .RemoveAsync(session.Id);
            }

            await _jwtBlacklistService.BlacklistTokenAsync(
                _currentUser.Jti,
                _currentUser.ExpiredAtString);
        }

        public async Task SendForgotPasswordOtpAsync(
        PlatformAdminForgotPasswordOtpRequest request)
        {
            var email = request.Email
                .Trim()
                .ToLowerInvariant();

            var platformAdmin =
                await _platformAdminRepository.GetByEmailAsync(
                    email);

            if (platformAdmin == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy Platform Admin.");
            }

            var account = new OtpAccount
            {
                Id = platformAdmin.Id,
                Email = platformAdmin.Email,
                AccountType = OtpAccountType.PlatformAdmin
            };

            await _otpService.SendForgotPasswordOtpAsync(
                account);
        }

        public async Task<PlatformAdminVerifyOtpResponse>VerifyForgotPasswordOtpAsync(
        PlatformAdminVerifyForgotPasswordOtpRequest request)
        {
            var email = request.Email
                .Trim()
                .ToLowerInvariant();

            var platformAdmin =
                await _platformAdminRepository.GetByEmailAsync(
                    email);

            if (platformAdmin == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy Platform Admin.");
            }

            var account = new OtpAccount
            {
                Id = platformAdmin.Id,
                Email = platformAdmin.Email,
                AccountType = OtpAccountType.PlatformAdmin
            };

            var result =
                await _otpService.VerifyForgotPasswordOtpAsync(
                    account,
                    request.Otp);

            return new PlatformAdminVerifyOtpResponse
            {
                VerificationToken = result.VerificationToken,
                ExpiredAt = result.ExpiredAt
            };
        }

        public async Task ForgotPasswordAsync(PlatformAdminForgotPasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                throw new BadRequestException(
                    "Mật khẩu xác nhận không trùng với mật khẩu mới.");
            }

            var email =
                await _otpService.ValidateForgotPasswordVerificationAsync(
                    request.VerificationToken,
                    OtpAccountType.PlatformAdmin);

            var platformAdmin =
                await _platformAdminRepository.GetByEmailAsync(
                    email);

            if (platformAdmin == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy Platform Admin.");
            }

            if (_passwordService.Verify(
                    request.NewPassword,
                    platformAdmin.PasswordHash))
            {
                throw new BadRequestException(
                    "Mật khẩu mới phải khác mật khẩu hiện tại.");
            }

            var sessions =
                await _platformAdminSessionRepository
                    .GetActiveByPlatformAdminIdAsync(
                        platformAdmin.Id);

            var revokedAt = DateTime.UtcNow;

            platformAdmin.PasswordHash =
                _passwordService.Hash(
                    request.NewPassword);

            platformAdmin.UpdatedAt = revokedAt;

            foreach (var session in sessions)
            {
                session.RevokedAt = revokedAt;
            }

            await _unitOfWork.SaveChangesAsync();

            foreach (var session in sessions)
            {
                await _platformAdminSessionCacheService
                    .RemoveAsync(session.Id);
            }
        }
    }
}
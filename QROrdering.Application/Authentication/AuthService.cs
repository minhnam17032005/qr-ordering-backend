using QROrdering.Application.Authentication.DTOs;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Application.Common.Interfaces;
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

        public AuthService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IJwtService jwtService,
            IUserSessionRepository userSessionRepository,
            IRequestInfoService requestInfoService,
            IUnitOfWork unitOfWork,
            IHashService hashService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _userSessionRepository = userSessionRepository;
            _unitOfWork = unitOfWork;
            _requestInfoService = requestInfoService;
            _hashService = hashService;
        }

        public async Task<RegisterResponse> RegisterAsync(
            RegisterRequest request)
        {
            // 1. Normalize input
            var fullName = request.FullName.Trim();
            var username = request.Username.Trim();
            var email = request.Email.Trim().ToLowerInvariant();

            // 2. Check duplicate username
            var usernameExists =
                await _userRepository.ExistsByUsernameAsync(username);

            // 3. Check duplicate email
            var emailExists =
                await _userRepository.ExistsByEmailAsync(email);

            // 4. Reject if username or email already exists
            if (usernameExists || emailExists)
            {
                throw new ConflictException(
                   "Username or email already exists.");
            }

            // 5. Hash password
            var passwordHash = _passwordService.Hash(request.Password);

            // 6. Create User
            var user = new User
            {
                FullName = fullName,
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                IsActive = true
            };

            // 7. Save User
            await _userRepository.AddAsync(user);

            // 8. Return response
            return new RegisterResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email
            };
        }

        public async Task<(LoginResponse response, string refreshToken)> LoginAsync(
     LoginRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            // Kiểm tra email + password
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null ||
                !_passwordService.Verify(
                    request.Password,
                    user.PasswordHash))
            {
                throw new UnauthorizedException(
                    "Email hoặc password không hợp lệ.");
            }

            // Kiểm tra tài khoản
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
                RefreshTokenHash =_hashService.Hash(refreshToken),

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

            // Add session
            await _userSessionRepository.AddAsync(session);

            // Commit transaction
            await _unitOfWork.SaveChangesAsync();

            var response = new LoginResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                AccessToken = accessToken
            };

            return (response, refreshToken);
        }
    }
}
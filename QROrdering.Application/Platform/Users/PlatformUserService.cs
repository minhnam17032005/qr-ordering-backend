using Microsoft.Extensions.Logging;
using QROrdering.Application.Authentication;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Exceptions;
using QROrdering.Application.Platform.Users.DTOs.Requests;
using QROrdering.Application.Platform.Users.DTOs.Responses;
using QROrdering.Application.Platform.Users.Interfaces;
using QROrdering.Domain.Entities.Identity;
using QROrdering.Domain.Entities.Membership;

namespace QROrdering.Application.Platform.Users
{
    public class PlatformUserService : IPlatformUserService
    {
        private readonly IPlatformUserRepository
            _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly ILogger<AuthService> _logger;
        private readonly ISessionCacheService _sessionCacheService;
        public PlatformUserService(
            IPlatformUserRepository userRepository,
            IPasswordService passwordService,
            IUnitOfWork unitOfWork,
            IUserSessionRepository userSessionRepository,
            ILogger<AuthService> logger,
            ISessionCacheService sessionCacheService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _unitOfWork = unitOfWork;
            _userSessionRepository = userSessionRepository;
            _logger = logger;
            _sessionCacheService = sessionCacheService;
        }

        public async Task<PagedResponse<PlatformUserListResponse>>GetUsersAsync(PagedRequest request)
        {
            var totalItems =
                await _userRepository.CountAsync();

            var users =
                await _userRepository.GetPagedAsync(
                    request.Page,
                    request.PageSize);

            var items = users
                .Select(MapToListResponse)
                .ToList();

            return new PagedResponse<PlatformUserListResponse>
            {
                Items = items,

                Pagination = new PaginationMeta
                {
                    Page = request.Page,

                    PageSize = request.PageSize,

                    TotalItems = totalItems,

                    TotalPages = (int)Math.Ceiling(
                        totalItems /
                        (double)request.PageSize)
                }
            };
        }

        public async Task<PlatformUserDetailResponse>GetUserByIdAsync(Guid userId)
        {
            var user =
                await _userRepository
                    .GetByIdWithRestaurantsAsync(
                        userId);

            if (user == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy người dùng.");
            }

            return MapToDetailResponse(user);
        }

        public async Task<PlatformUserDetailResponse>CreateUserAsync(CreatePlatformUserRequest request)
        {
            if (request.Password != request.ConfirmPassword)
            {
                throw new BadRequestException(
                    "Mật khẩu xác nhận không trùng với mật khẩu.");
            }

            var username =
                request.Username.Trim().ToLowerInvariant();

            var email =
                request.Email.Trim().ToLowerInvariant();

            var fullName =
                request.FullName.Trim();

            var phoneNumber =
                request.PhoneNumber?.Trim();

            var avatarUrl =
                request.AvatarUrl?.Trim();

            if (await _userRepository
                .ExistsByUsernameAsync(username))
            {
                throw new ConflictException(
                    "Username đã tồn tại.");
            }

            if (await _userRepository
                .ExistsByEmailAsync(email))
            {
                throw new ConflictException(
                    "Email đã tồn tại.");
            }

            var user = new User
            {
                Username = username,
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                AvatarUrl = avatarUrl,

                PasswordHash =
                    _passwordService.Hash(
                        request.Password),

                IsActive = true
            };

            await _userRepository.AddAsync(user);

            await _unitOfWork.SaveChangesAsync();

            return MapToDetailResponse(user);
        }

        public async Task<PlatformUserDetailResponse>UpdateUserAsync(
        Guid userId,
        UpdatePlatformUserRequest request)
        {
            var user =
                await _userRepository
                    .GetByIdWithRestaurantsAsync(userId);

            if (user == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy người dùng.");
            }

            var username =
                request.Username.Trim().ToLowerInvariant();

            var email =
                request.Email.Trim().ToLowerInvariant();

            var fullName =
                request.FullName.Trim();

            var phoneNumber =
                request.PhoneNumber?.Trim();

            var avatarUrl =
                request.AvatarUrl?.Trim();

            if (await _userRepository
                .ExistsByUsernameExceptAsync(
                    username,
                    userId))
            {
                throw new ConflictException(
                    "Username đã tồn tại.");
            }

            if (await _userRepository
                .ExistsByEmailExceptAsync(
                    email,
                    userId))
            {
                throw new ConflictException(
                    "Email đã tồn tại.");
            }

            user.Username = username;

            user.FullName = fullName;

            user.Email = email;

            user.PhoneNumber = phoneNumber;

            user.AvatarUrl = avatarUrl;

            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return MapToDetailResponse(user);
        }

        public async Task ActivateUserAsync(
        Guid userId)
        {
            var user =
                await _userRepository
                    .GetByIdForUpdateAsync(userId);

            if (user == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy người dùng.");
            }

            if (user.IsActive)
            {
                throw new BadRequestException(
                    "Người dùng đã được kích hoạt.");
            }

            user.IsActive = true;

            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateUserAsync(
        Guid userId)
        {
            var user =
                await _userRepository
                    .GetByIdForUpdateAsync(userId);

            if (user == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy người dùng.");
            }

            if (!user.IsActive)
            {
                throw new BadRequestException(
                    "Người dùng đã bị vô hiệu hóa.");
            }

            var sessions =
                await _userSessionRepository
                    .GetActiveByUserIdAsync(userId);

            var revokedAt = DateTime.UtcNow;

            user.IsActive = false;

            user.UpdatedAt = revokedAt;

            foreach (var session in sessions)
            {
                session.RevokedAt = revokedAt;
            }

            await _unitOfWork.SaveChangesAsync();

            foreach (var session in sessions)
            {
                await _sessionCacheService
                    .RemoveAsync(session.Id);
            }

            _logger.LogInformation(
                "User {UserId} was deactivated. " +
                "Revoked {SessionCount} active sessions.",
                userId,
                sessions.Count);
        }


        private static PlatformUserListResponse
            MapToListResponse(
                User user)
        {
            return new PlatformUserListResponse
            {
                Id = user.Id,

                Username = user.Username,

                FullName = user.FullName,

                Email = user.Email,

                IsActive = user.IsActive,

                Restaurants = user.RestaurantMembers
                    .Where(x => x.Restaurant != null)
                    .Select(MapToRestaurantResponse)
                    .ToList()
            };
        }

        private static PlatformUserDetailResponse
            MapToDetailResponse(
                User user)
        {
            return new PlatformUserDetailResponse
            {
                Id = user.Id,

                Username = user.Username,

                FullName = user.FullName,

                Email = user.Email,

                PhoneNumber = user.PhoneNumber,

                AvatarUrl = user.AvatarUrl,

                IsActive = user.IsActive,

                CreatedAt = user.CreatedAt,

                UpdatedAt = user.UpdatedAt,

                Restaurants = user.RestaurantMembers
                    .Where(x => x.Restaurant != null)
                    .Select(MapToRestaurantResponse)
                    .ToList()
            };
        }

        private static PlatformUserRestaurantResponse
            MapToRestaurantResponse(
                RestaurantMember member)
        {
            return new PlatformUserRestaurantResponse
            {
                RestaurantId =
                    member.Restaurant.Id,

                RestaurantName =
                    member.Restaurant.Name,

                IsActive =
                    member.Restaurant.IsActive,

                Roles = member.MemberRoles
                    .Where(x => x.Role != null)
                    .Select(x => x.Role.Name)
                    .ToList()
            };
        }
    }
}

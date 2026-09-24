using System.Data;
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Application.Authorization.Interfaces;
using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Exceptions;
using QROrdering.Application.Membership.Interfaces;
using QROrdering.Application.Platform.Registrations.DTOs.Responses;
using QROrdering.Application.Platform.Registrations.Interfaces;
using QROrdering.Application.Platform.Restaurants.Interfaces;
using QROrdering.Domain.Entities.Authorization;
using QROrdering.Domain.Entities.Membership;
using QROrdering.Domain.Entities.Platform;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Domain.Enums;

namespace QROrdering.Application.Platform.Registrations
{
    public class PlatformServiceRegistrationService
        : IPlatformServiceRegistrationService
    {
        private readonly IPlatformServiceRegistrationRepository _registrationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IPlatformRestaurantRepository _restaurantRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRestaurantMemberRepository _restaurantMemberRepository;
        private readonly IMemberRoleRepository _memberRoleRepository;

        public PlatformServiceRegistrationService(
            IPlatformServiceRegistrationRepository registrationRepository,
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            ICurrentUserService currentUser,
            IRoleRepository roleRepository,
            IPlatformRestaurantRepository restaurantRepository,
            IRestaurantMemberRepository restaurantMemberRepository,
            IMemberRoleRepository memberRoleRepository
            )
        {
            _registrationRepository = registrationRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _currentUser = currentUser;
            _roleRepository = roleRepository;
            _restaurantMemberRepository = restaurantMemberRepository;
            _memberRoleRepository = memberRoleRepository;
            _restaurantRepository = restaurantRepository;
        }

        public async Task<PagedResponse<PlatformServiceRegistrationListResponse>>GetRegistrationsAsync(
                PagedRequest request)
        {
            var totalItems =
                await _registrationRepository.CountAsync();

            var registrations =
                await _registrationRepository.GetPagedAsync(
                    request.Page,
                    request.PageSize);

            var items = registrations
                .Select(x =>
                    new PlatformServiceRegistrationListResponse
                    {
                        Id = x.Id,
                        ContactName = x.ContactName,
                        RestaurantName = x.RestaurantName,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        Status = x.Status.ToString(),
                        CreatedAt = x.CreatedAt
                    })
                .ToList();

            var totalPages =
                (int)Math.Ceiling(
                    (double)totalItems /
                    request.PageSize);

            return new PagedResponse<
                PlatformServiceRegistrationListResponse>
            {
                Items = items,

                Pagination = new PaginationMeta
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages
                }
            };
        }

        public async Task<PlatformServiceRegistrationDetailResponse>GetRegistrationByIdAsync(Guid registrationId)
        {
            var registration =
                await _registrationRepository
                    .GetByIdWithDetailsAsync(
                        registrationId);

            if (registration == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy đơn đăng ký.");
            }

            return new PlatformServiceRegistrationDetailResponse
            {
                Id = registration.Id,

                // Người đăng ký

                UserId = registration.UserId,
                ContactName = registration.ContactName,
                Email = registration.Email,
                PhoneNumber = registration.PhoneNumber,


                // Thông tin nhà hàng

                RestaurantName = registration.RestaurantName,
                RestaurantAddress = registration.RestaurantAddress,
                RestaurantPhoneNumber = registration.RestaurantPhoneNumber,
                RestaurantEmail = registration.RestaurantEmail,
                RestaurantDescription = registration.RestaurantDescription,
                RestaurantLogoUrl = registration.RestaurantLogoUrl,


                // Trạng thái

                Status = registration.Status.ToString(),
                Note = registration.Note,


                // Platform xử lý

                ProcessedByPlatformAdminId =
                    registration.ProcessedByPlatformAdminId,
                ProcessedAt =
                    registration.ProcessedAt,


                // Restaurant

                RestaurantId = registration.RestaurantId,

                CreatedAt = registration.CreatedAt,
                UpdatedAt = registration.UpdatedAt
            };
        }

        public async Task ApproveRegistrationAsync(Guid registrationId)
        {
            var registration =
                await _registrationRepository
                    .GetByIdForUpdateAsync(
                        registrationId);

            if (registration == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy đơn đăng ký.");
            }

            if (registration.Status !=
                ServiceRegistrationStatus.Pending)
            {
                throw new BadRequestException(
                    "Đơn đăng ký không ở trạng thái Pending.");
            }

            var platformAdminId =
                _currentUser.UserId;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var now = DateTime.UtcNow;

                // ========================================
                // 1. Create Restaurant
                // ========================================

                var restaurant = new Restaurant
                {
                    Id = Guid.NewGuid(),

                    Name = registration.RestaurantName,

                    Address = registration.RestaurantAddress,

                    PhoneNumber =
                        registration.RestaurantPhoneNumber
                        ?? registration.PhoneNumber
                        ?? string.Empty,

                    Email =
                        registration.RestaurantEmail
                        ?? registration.Email,

                    Description =
                        registration.RestaurantDescription,

                    LogoUrl =
                        registration.RestaurantLogoUrl,

                    IsActive = true
                };

                await _restaurantRepository
                    .AddAsync(restaurant);


                // ========================================
                // 2. Create ALL default RoleTypes
                // ========================================

                var roles = new List<Role>();

                foreach (var roleType in Enum.GetValues<RoleType>())
                {
                    var roleInfo = GetRoleInfo(roleType);

                    var role = new Role
                    {
                        Id = Guid.NewGuid(),

                        RestaurantId = restaurant.Id,

                        Type = roleType,

                        Name = roleInfo.Name,

                        Description = roleInfo.Description,

                        IsActive = true
                    };

                    await _roleRepository
                        .AddAsync(role);

                    roles.Add(role);
                }

                // ========================================
                // 3. Find Admin Role
                // ========================================

                var adminRole =
                    roles.FirstOrDefault(
                        x => x.Type == RoleType.Admin);

                if (adminRole == null)
                {
                    throw new BadRequestException(
                        "Không tìm thấy Role Admin.");
                }


                // ========================================
                // 4. Create RestaurantMember
                // ========================================

                var restaurantMember =
                    new RestaurantMember
                    {
                        Id = Guid.NewGuid(),

                        UserId = registration.UserId,

                        RestaurantId = restaurant.Id,

                        IsActive = true
                    };

                await _restaurantMemberRepository
                    .AddAsync(restaurantMember);


                // ========================================
                // 5. Assign Admin Role to Owner
                // ========================================

                var memberRole =
                    new MemberRole
                    {
                        Id = Guid.NewGuid(),

                        RestaurantId = restaurant.Id,

                        RestaurantMemberId =
                            restaurantMember.Id,

                        RoleId = adminRole.Id
                    };

                await _memberRoleRepository
                    .AddAsync(memberRole);


                // ========================================
                // 6. Update ServiceRegistration
                // ========================================

                registration.Status =
                    ServiceRegistrationStatus.Approved;

                registration.RestaurantId =
                    restaurant.Id;

                registration.ProcessedByPlatformAdminId =
                    platformAdminId;

                registration.ProcessedAt =
                    now;

                registration.UpdatedAt =
                    now;

                _registrationRepository
                    .Update(registration);


                // ========================================
                // 7. Save everything
                // ========================================

                await _unitOfWork
                    .SaveChangesAsync();


                // ========================================
                // 8. Commit
                // ========================================

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();

                throw;
            }
        }

        public async Task RejectRegistrationAsync(Guid registrationId)
        {
            var registration =
                await _registrationRepository
                    .GetByIdForUpdateAsync(
                        registrationId);

            if (registration == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy đơn đăng ký.");
            }

            if (registration.Status !=
                ServiceRegistrationStatus.Pending)
            {
                throw new BadRequestException(
                    "Đơn đăng ký không ở trạng thái Pending.");
            }

            var platformAdminId =
                _currentUser.UserId;

            var now = DateTime.UtcNow;

            // ========================================
            // 1. Update ServiceRegistration
            // ========================================

            registration.Status =
                ServiceRegistrationStatus.Rejected;

            registration.ProcessedByPlatformAdminId =
                platformAdminId;

            registration.ProcessedAt =
                now;

            registration.UpdatedAt =
                now;


            // ========================================
            // 2. Mark as Modified
            // ========================================

            _registrationRepository
                .Update(registration);


            // ========================================
            // 3. Save
            // ========================================

            await _unitOfWork
                .SaveChangesAsync();
        }

        private static (string Name,string Description) GetRoleInfo(
        RoleType roleType)
        {
            return roleType switch
            {
                RoleType.Admin => (
                    "Admin",
                    "Quản trị nhà hàng"),

                RoleType.Staff => (
                    "Staff",
                    "Nhân viên nhà hàng"),

                _ => (
                    roleType.ToString(),
                    string.Empty)
            };
        }
    }
}
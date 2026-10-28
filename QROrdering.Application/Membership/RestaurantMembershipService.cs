
using QROrdering.Application.Authentication.Interfaces;
using QROrdering.Application.Authorization.Interfaces;
using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Exceptions;
using QROrdering.Application.Membership.DTOs.Requests;
using QROrdering.Application.Membership.DTOs.Responses;
using QROrdering.Application.Membership.Enums;
using QROrdering.Application.Membership.Interfaces;
using QROrdering.Domain.Entities.Identity;
using QROrdering.Domain.Entities.Membership;
using QROrdering.Domain.Enums;
using QROrdering.Infrastructure.Membership.DTOs.Requests;
using QROrdering.Infrastructure.Membership.DTOs.Responses;

namespace QROrdering.Application.Membership
{
    public class RestaurantMembershipService
        : IRestaurantMembershipService
    {
        private readonly IRestaurantMemberRepository _restaurantMemberRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IRoleRepository _roleRepository;
        private readonly IMemberRoleRepository _memberRoleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;

        public RestaurantMembershipService(
            IRestaurantMemberRepository restaurantMemberRepository,
            ICurrentUserService currentUser,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IMemberRoleRepository memberRoleRepository,
            IUnitOfWork unitOfWork,
            IPasswordService passwordService
            )
        {
            _restaurantMemberRepository = restaurantMemberRepository;
            _currentUser = currentUser;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _memberRoleRepository = memberRoleRepository;
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;

        }

        // ============================================================
        // GET MEMBERS
        // ============================================================

        public async Task<PagedResponse<RestaurantMemberListResponse>>GetMembersAsync(
                Guid restaurantId,
                PagedRequest request)
        {
            if (restaurantId == Guid.Empty)
            {
                throw new BadRequestException(
                    "RestaurantId không hợp lệ.");
            }

            if (request.Page < 1 || request.PageSize < 1)
            {
                throw new BadRequestException(
                    "Page và PageSize phải lớn hơn hoặc bằng 1.");
            }

            var isAdmin =
                await _restaurantMemberRepository.IsActiveAdminAsync(
                    _currentUser.UserId,
                    restaurantId);

            if (!isAdmin)
            {
                throw new ForbiddenException(
                    "Bạn không có quyền quản lý thành viên của nhà hàng này.");
            }

            var totalItems =
                await _restaurantMemberRepository.CountByRestaurantAsync(
                    restaurantId);

            var members =
                await _restaurantMemberRepository
                    .GetPagedByRestaurantAsync(
                        restaurantId,
                        request.Page,
                        request.PageSize);

            var items = members
                .Select(member => new RestaurantMemberListResponse
                {
                    Id = member.Id,
                    UserId = member.UserId,
                    Username = member.User.Username,
                    FullName = member.User.FullName,
                    Email = member.User.Email,
                    IsActive = member.IsActive,
                    CreatedAt = member.CreatedAt,

                    Roles = member.MemberRoles
                        .Where(memberRole =>
                            memberRole.RestaurantId == restaurantId &&
                            memberRole.Role.RestaurantId == restaurantId &&
                            memberRole.Role.IsActive)
                        .Select(memberRole => memberRole.Role.Name)
                        .Distinct()
                        .ToList()
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(
                (double)totalItems / request.PageSize);

            return new PagedResponse<RestaurantMemberListResponse>
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

        // ============================================================
        // GET MEMBER BY ID
        // ============================================================

        public async Task<RestaurantMemberDetailResponse>GetMemberByIdAsync(
                Guid restaurantId,
                Guid memberId)
        {
            if (restaurantId == Guid.Empty ||
                memberId == Guid.Empty)
            {
                throw new BadRequestException(
                    "RestaurantId hoặc MemberId không hợp lệ.");
            }

            var isAdmin =
                await _restaurantMemberRepository.IsActiveAdminAsync(
                    _currentUser.UserId,
                    restaurantId);

            if (!isAdmin)
            {
                throw new ForbiddenException(
                    "Bạn không có quyền quản lý thành viên của nhà hàng này.");
            }

            var member =
                await _restaurantMemberRepository
                    .GetByIdInRestaurantAsync(
                        restaurantId,
                        memberId);

            if (member == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy thành viên trong nhà hàng này.");
            }

            return new RestaurantMemberDetailResponse
            {
                Id = member.Id,
                UserId = member.UserId,
                Username = member.User.Username,
                FullName = member.User.FullName,
                Email = member.User.Email,
                PhoneNumber = member.User.PhoneNumber,
                AvatarUrl = member.User.AvatarUrl,
                IsActive = member.IsActive,
                CreatedAt = member.CreatedAt,
                UpdatedAt = member.UpdatedAt,

                Roles = member.MemberRoles
                    .Where(memberRole =>
                        memberRole.RestaurantId == restaurantId &&
                        memberRole.Role.RestaurantId == restaurantId &&
                        memberRole.Role.IsActive)
                    .Select(memberRole => memberRole.Role.Name)
                    .Distinct()
                    .ToList()
            };
        }



        public async Task<UserLookupResponse> LookupUserAsync(
            Guid restaurantId,
            UserLookupRequest request)
        {
            // 1. Validate input
            if (restaurantId == Guid.Empty)
            {
                throw new BadRequestException(
                    "RestaurantId không hợp lệ.");
            }

            if (request is null ||
                !Enum.IsDefined(typeof(UserLookupType), request.SearchType) ||
                string.IsNullOrWhiteSpace(request.Keyword))
            {
                throw new BadRequestException(
                    "Loại tìm kiếm hoặc từ khóa không hợp lệ.");
            }

            var keyword = request.Keyword.Trim();

            // 2. Validate search criteria
            Guid? userId = null;
            string? email = null;
            string? phoneNumber = null;

            switch (request.SearchType)
            {
                case UserLookupType.UserId:
                    if (!Guid.TryParse(keyword, out var parsedUserId) ||
                        parsedUserId == Guid.Empty)
                    {
                        throw new BadRequestException(
                            "UserId không hợp lệ.");
                    }

                    userId = parsedUserId;
                    break;

                case UserLookupType.Email:
                    email = keyword.ToLowerInvariant();
                    break;

                case UserLookupType.PhoneNumber:
                    phoneNumber = keyword;
                    break;

                default:
                    throw new BadRequestException(
                        "Loại tìm kiếm không hợp lệ.");
            }

            // 3. Check Admin permission for this restaurant
            var isAdmin = await _restaurantMemberRepository
                .IsActiveAdminAsync(
                    _currentUser.UserId,
                    restaurantId);

            if (!isAdmin)
            {
                throw new ForbiddenException(
                    "Bạn không có quyền tra cứu người dùng của nhà hàng này.");
            }

            // 4. Find global User account
            var user = await _userRepository.GetByLookupAsync(
                userId,
                email,
                phoneNumber);

            if (user is null)
            {
                throw new NotFoundException(
                    "Không tìm thấy tài khoản người dùng.");
            }

            // 5. Return User information
            return new UserLookupResponse
            {
                UserId = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive
            };
        }



        public async Task<AddMemberResponse> AddMemberAsync(
            Guid restaurantId,
            AddMemberRequest request)
        {
            if (restaurantId == Guid.Empty)
            {
                throw new BadRequestException(
                    "RestaurantId không hợp lệ.");
            }

            if (request is null)
            {
                throw new BadRequestException(
                    "Dữ liệu thêm thành viên không được để trống.");
            }

            if (request.UserId == Guid.Empty)
            {
                throw new BadRequestException(
                    "UserId không hợp lệ.");
            }

            if (request.RoleIds is null || request.RoleIds.Count == 0)
            {
                throw new BadRequestException(
                    "Phải chọn ít nhất một vai trò.");
            }

            if (request.RoleIds.Any(id => id == Guid.Empty))
            {
                throw new BadRequestException(
                    "RoleIds không được chứa Guid.Empty.");
            }

            if (request.RoleIds.Distinct().Count() != request.RoleIds.Count)
            {
                throw new BadRequestException(
                    "RoleIds không được chứa ID trùng nhau.");
            }

            // 1. Người gọi phải là Admin active của nhà hàng đích.
            var isAdmin = await _restaurantMemberRepository.IsActiveAdminAsync(
                _currentUser.UserId,
                restaurantId);

            if (!isAdmin)
            {
                throw new ForbiddenException(
                    "Bạn không có quyền quản lý thành viên của nhà hàng này.");
            }

            // 2. Kiểm tra User đích.
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user is null)
            {
                throw new NotFoundException(
                    "Không tìm thấy tài khoản người dùng.");
            }

            if (!user.IsActive)
            {
                throw new BadRequestException(
                    "Không thể thêm tài khoản người dùng đã bị vô hiệu hóa.");
            }

            // 3. Không tạo membership trùng.
            var existingMember =
                await _restaurantMemberRepository.GetByUserIdAndRestaurantIdAsync(
                    request.UserId,
                    restaurantId);

            if (existingMember is not null)
            {
                throw new ConflictException(
                    existingMember.IsActive
                        ? "Người dùng đã là thành viên của nhà hàng này."
                        : "Người dùng đã có membership không hoạt động. Hãy sử dụng API kích hoạt.");
            }

            // 4. Kiểm tra toàn bộ Role trước khi ghi dữ liệu.
            var roles = await _roleRepository.GetByIdsInRestaurantAsync(
                restaurantId,
                request.RoleIds);

            if (roles.Count != request.RoleIds.Count)
            {
                throw new BadRequestException(
                    "Có vai trò không tồn tại hoặc không thuộc nhà hàng này.");
            }

            if (roles.Any(role => !role.IsActive))
            {
                throw new BadRequestException(
                    "Không thể gán vai trò đã bị vô hiệu hóa.");
            }

            // 5. Tạo membership và các MemberRole trong cùng transaction.
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var member = new RestaurantMember
                {
                    UserId = user.Id,
                    RestaurantId = restaurantId,
                    IsActive = true
                };

                await _restaurantMemberRepository.AddAsync(member);

                // Lưu trong transaction để lấy member.Id.
                await _unitOfWork.SaveChangesAsync();

                foreach (var role in roles)
                {
                    await _memberRoleRepository.AddAsync(new MemberRole
                    {
                        RestaurantId = restaurantId,
                        RestaurantMemberId = member.Id,
                        RoleId = role.Id
                    });
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new AddMemberResponse
                {
                    MemberId = member.Id,
                    UserId = member.UserId,
                    RestaurantId = member.RestaurantId,
                    IsActive = member.IsActive,
                    Roles = roles.Select(role => new AssignedRoleResponse
                    {
                        RoleId = role.Id,
                        RoleName = role.Name
                    }).ToList()
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }


        public async Task<RegisterMemberResponse> RegisterMemberAsync(
            Guid restaurantId,
            RegisterMemberRequest request)
        {
            if (restaurantId == Guid.Empty)
            {
                throw new BadRequestException(
                    "RestaurantId không hợp lệ.");
            }

            if (request is null)
            {
                throw new BadRequestException(
                    "Dữ liệu đăng ký không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                throw new BadRequestException(
                    "FullName, Username, Email và Password là bắt buộc.");
            }

            request.FullName = request.FullName.Trim();
            request.Username = request.Username.Trim().ToLowerInvariant();
            request.Email = request.Email.Trim().ToLowerInvariant();

            if (request.PhoneNumber is not null)
            {
                request.PhoneNumber = request.PhoneNumber.Trim();

                if (request.PhoneNumber.Length == 0)
                {
                    request.PhoneNumber = null;
                }
            }

            if (request.RoleIds is null || request.RoleIds.Count == 0)
            {
                throw new BadRequestException(
                    "Phải chọn ít nhất một vai trò.");
            }

            if (request.RoleIds.Any(id => id == Guid.Empty))
            {
                throw new BadRequestException(
                    "RoleIds không được chứa Guid.Empty.");
            }

            if (request.RoleIds.Distinct().Count() != request.RoleIds.Count)
            {
                throw new BadRequestException(
                    "RoleIds không được chứa ID trùng nhau.");
            }

            // 1. Người gọi phải là Admin active của nhà hàng đích.
            var isAdmin = await _restaurantMemberRepository.IsActiveAdminAsync(
                _currentUser.UserId,
                restaurantId);

            if (!isAdmin)
            {
                throw new ForbiddenException(
                    "Bạn không có quyền quản lý thành viên của nhà hàng này.");
            }

            // 2. Kiểm tra nhà hàng tồn tại.
            // Bổ sung repository kiểm tra Restaurant nếu service hiện tại chưa có.

            // 3. Kiểm tra trùng Username, Email, PhoneNumber.
            var exists = await _userRepository
                .ExistsByUsernameOrEmailOrPhoneAsync(
                    request.Username,
                    request.Email,
                    request.PhoneNumber);

            if (exists)
            {
                throw new ConflictException(
                    "Username, Email hoặc PhoneNumber đã được sử dụng.");
            }

            // 4. Kiểm tra toàn bộ Role trước khi ghi dữ liệu.
            var roles = await _roleRepository.GetByIdsInRestaurantAsync(
                restaurantId,
                request.RoleIds);

            if (roles.Count != request.RoleIds.Count)
            {
                throw new BadRequestException(
                    "Có vai trò không tồn tại hoặc không thuộc nhà hàng này.");
            }

            if (roles.Any(role => !role.IsActive))
            {
                throw new BadRequestException(
                    "Không thể gán vai trò đã bị vô hiệu hóa.");
            }

            // 5. Tạo User + RestaurantMember + MemberRole trong cùng transaction.
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var user = new User
                {
                    FullName = request.FullName,
                    Username = request.Username,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    PasswordHash = _passwordService.Hash(request.Password),
                    IsActive = true
                };

                // UserRepository.AddAsync hiện tự SaveChangesAsync.
                // Lệnh này vẫn nằm trong transaction đã mở ở trên.
                await _userRepository.AddAsync(user);

                var member = new RestaurantMember
                {
                    UserId = user.Id,
                    RestaurantId = restaurantId,
                    IsActive = true
                };

                await _restaurantMemberRepository.AddAsync(member);
                await _unitOfWork.SaveChangesAsync();

                foreach (var role in roles)
                {
                    await _memberRoleRepository.AddAsync(new MemberRole
                    {
                        RestaurantId = restaurantId,
                        RestaurantMemberId = member.Id,
                        RoleId = role.Id
                    });
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new RegisterMemberResponse
                {
                    UserId = user.Id,
                    MemberId = member.Id,
                    RestaurantId = restaurantId,
                    FullName = user.FullName,
                    Username = user.Username,
                    Email = user.Email,
                    IsActive = member.IsActive,
                    Roles = roles.Select(role => new AssignedRoleResponse
                    {
                        RoleId = role.Id,
                        RoleName = role.Name
                    }).ToList()
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<UpdateMemberRolesResponse> UpdateMemberRolesAsync(
            Guid restaurantId,
            Guid memberId,
            UpdateMemberRolesRequest request)
        {
            if (restaurantId == Guid.Empty)
                throw new BadRequestException("RestaurantId không hợp lệ.");

            if (memberId == Guid.Empty)
                throw new BadRequestException("MemberId không hợp lệ.");

            if (request is null)
                throw new BadRequestException("Dữ liệu yêu cầu không hợp lệ.");

            if (request.RoleIds is null || request.RoleIds.Count == 0)
                throw new BadRequestException("Phải chọn ít nhất một Role.");

            if (request.RoleIds.Any(id => id == Guid.Empty))
                throw new BadRequestException("RoleId không hợp lệ.");

            if (request.RoleIds.Distinct().Count() != request.RoleIds.Count)
                throw new BadRequestException("Danh sách Role không được trùng lặp.");

            // 1. Người gọi phải là Admin đang hoạt động của nhà hàng.
            var isAdmin = await _restaurantMemberRepository.IsActiveAdminAsync(
                _currentUser.UserId,
                restaurantId);

            if (!isAdmin)
                throw new ForbiddenException(
                    "Bạn không có quyền quản lý thành viên của nhà hàng này.");

            // 2. Member phải tồn tại trong đúng nhà hàng.
            var member = await _restaurantMemberRepository
                .GetByIdInRestaurantAsync(restaurantId, memberId);

            if (member is null)
                throw new NotFoundException("Không tìm thấy thành viên.");

            if (!member.IsActive)
                throw new ConflictException(
                    "Không thể cập nhật Role cho thành viên đã bị vô hiệu hóa.");

            if (!member.User.IsActive)
                throw new ConflictException(
                    "Không thể cập nhật Role cho tài khoản đã bị vô hiệu hóa.");

            // 3. Tất cả Role phải thuộc đúng nhà hàng và đang hoạt động.
            var requestedRoles = await _roleRepository.GetByIdsInRestaurantAsync(
                restaurantId,
                request.RoleIds);

            if (requestedRoles.Count != request.RoleIds.Count)
                throw new BadRequestException(
                    "Một hoặc nhiều Role không tồn tại trong nhà hàng.");

            if (requestedRoles.Any(role => !role.IsActive))
                throw new ConflictException(
                    "Không thể gán Role đã bị vô hiệu hóa.");

            // 4. Lấy các liên kết Role hiện tại.
            var existingMemberRoles = await _memberRoleRepository
                .GetByMemberIdAsync(restaurantId, memberId);

            var requestedRoleIds = request.RoleIds.ToHashSet();
            var existingRoleIds = existingMemberRoles
                .Select(x => x.RoleId)
                .ToHashSet();

            // Chỉ thêm Role chưa có và xóa Role không còn được yêu cầu.
            var rolesToRemove = existingMemberRoles
                .Where(x => !requestedRoleIds.Contains(x.RoleId))
                .ToList();

            var roleIdsToAdd = requestedRoleIds
                .Except(existingRoleIds)
                .ToList();

            // 5. Không cho phép loại bỏ Admin cuối cùng của nhà hàng.
            var currentlyHasAdminRole = existingMemberRoles.Any(x =>
                x.Role.Type == RoleType.Admin &&
                x.Role.IsActive &&
                x.Role.RestaurantId == restaurantId);

            var willHaveAdminRole = requestedRoles.Any(role =>
                role.Type == RoleType.Admin &&
                role.IsActive &&
                role.RestaurantId == restaurantId);

            if (currentlyHasAdminRole && !willHaveAdminRole)
            {
                var activeAdminCount =
                    await _restaurantMemberRepository.CountActiveAdminsAsync(
                        restaurantId);

                if (activeAdminCount <= 1)
                    throw new ConflictException(
                        "Không thể loại bỏ Role Admin của Admin cuối cùng.");
            }

            // 6. Cập nhật trong một transaction.
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                if (rolesToRemove.Count > 0)
                    _memberRoleRepository.RemoveRange(rolesToRemove);

                foreach (var roleId in roleIdsToAdd)
                {
                    await _memberRoleRepository.AddAsync(new MemberRole
                    {
                        RestaurantId = restaurantId,
                        RestaurantMemberId = memberId,
                        RoleId = roleId
                    });
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            // 7. Trả về Role cuối cùng theo yêu cầu.
            return new UpdateMemberRolesResponse
            {
                MemberId = memberId,
                RestaurantId = restaurantId,
                Roles = requestedRoles
                    .Select(role => new AssignedRoleResponse
                    {
                        RoleId = role.Id,
                        RoleName = role.Name
                    })
                    .ToList()
            };
        }

        public async Task<ActivateMemberResponse> ActivateMemberAsync(
            Guid restaurantId,
            Guid memberId)
        {
            if (restaurantId == Guid.Empty || memberId == Guid.Empty)
            {
                throw new BadRequestException(
                    "RestaurantId và MemberId không hợp lệ.");
            }

            // Chỉ Admin active của nhà hàng đích mới được phép.
            var isAdmin = await _restaurantMemberRepository.IsActiveAdminAsync(
                _currentUser.UserId,
                restaurantId);

            if (!isAdmin)
            {
                throw new ForbiddenException(
                    "Bạn không có quyền kích hoạt thành viên tại nhà hàng này.");
            }

            // Truy vấn đồng thời theo RestaurantId và MemberId,
            // không lấy membership từ nhà hàng khác.
            var member = await _restaurantMemberRepository
                .GetByIdInRestaurantAsync(restaurantId, memberId);

            if (member == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy thành viên trong nhà hàng.");
            }

            // Idempotent: đã active thì trả kết quả hiện tại,
            // không cập nhật DB hoặc thay đổi Role.
            if (member.IsActive)
            {
                return new ActivateMemberResponse
                {
                    MemberId = member.Id,
                    UserId = member.UserId,
                    RestaurantId = member.RestaurantId,
                    IsActive = member.IsActive
                };
            }

            // Không tự động kích hoạt tài khoản User toàn cục.
            if (!member.User.IsActive)
            {
                throw new ConflictException(
                    "Không thể kích hoạt thành viên vì tài khoản người dùng đang bị vô hiệu hóa.");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                member.IsActive = true;

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new ActivateMemberResponse
                {
                    MemberId = member.Id,
                    UserId = member.UserId,
                    RestaurantId = member.RestaurantId,
                    IsActive = member.IsActive
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<DeactivateMemberResponse> DeactivateMemberAsync(
            Guid restaurantId,
            Guid memberId)
        {
            if (restaurantId == Guid.Empty || memberId == Guid.Empty)
            {
                throw new BadRequestException(
                    "RestaurantId và MemberId không hợp lệ.");
            }

            var isAdmin = await _restaurantMemberRepository.IsActiveAdminAsync(
                _currentUser.UserId,
                restaurantId);

            if (!isAdmin)
            {
                throw new ForbiddenException(
                    "Bạn không có quyền vô hiệu hóa thành viên tại nhà hàng này.");
            }

            var member = await _restaurantMemberRepository
                .GetByIdInRestaurantAsync(restaurantId, memberId);

            if (member == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy thành viên trong nhà hàng.");
            }

            // Idempotent: không cập nhật lại membership đã inactive.
            if (!member.IsActive)
            {
                return new DeactivateMemberResponse
                {
                    MemberId = member.Id,
                    UserId = member.UserId,
                    RestaurantId = member.RestaurantId,
                    IsActive = false
                };
            }

            // Chỉ cần kiểm tra quy tắc Admin cuối cùng
            // khi member hiện có Role Admin đang active.
            var hasActiveAdminRole = member.MemberRoles.Any(memberRole =>
                memberRole.RestaurantId == restaurantId &&
                memberRole.Role.RestaurantId == restaurantId &&
                memberRole.Role.IsActive &&
                memberRole.Role.Type == RoleType.Admin);

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                if (hasActiveAdminRole)
                {
                    var activeAdminCount =
                        await _restaurantMemberRepository
                            .CountActiveAdminsAsync(restaurantId);

                    if (activeAdminCount <= 1)
                    {
                        throw new ConflictException(
                            "Không thể vô hiệu hóa Admin cuối cùng của nhà hàng.");
                    }
                }

                member.IsActive = false;

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new DeactivateMemberResponse
                {
                    MemberId = member.Id,
                    UserId = member.UserId,
                    RestaurantId = member.RestaurantId,
                    IsActive = member.IsActive
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<List<RestaurantRoleResponse>> GetRolesAsync(
            Guid restaurantId)
        {
            if (restaurantId == Guid.Empty)
            {
                throw new BadRequestException(
                    "RestaurantId không hợp lệ.");
            }

            var isAdmin = await _restaurantMemberRepository.IsActiveAdminAsync(
                _currentUser.UserId,
                restaurantId);

            if (!isAdmin)
            {
                throw new ForbiddenException(
                    "Bạn không có quyền xem danh sách Role của nhà hàng này.");
            }

            var roles = await _roleRepository
                .GetActiveByRestaurantIdAsync(restaurantId);

            return roles.Select(role => new RestaurantRoleResponse
            {
                RoleId = role.Id,
                Name = role.Name,
                Description = role.Description,
                Type = (int)role.Type,
                IsActive = role.IsActive
            }).ToList();
        }

        public async Task<List<AccountRestaurantResponse>>GetMyRestaurantsAsync()
        {
            var userId = _currentUser.UserId;

            if (userId == Guid.Empty)
            {
                throw new UnauthorizedException(
                    "Không xác định được tài khoản đăng nhập.");
            }

            // Chỉ lấy membership của User đang đăng nhập.
            var memberships = await _restaurantMemberRepository
                .GetActiveMembershipsWithDetailsByUserIdAsync(userId);

            return memberships
                .Select(member => new AccountRestaurantResponse
                {
                    RestaurantId = member.RestaurantId,
                    Name = member.Restaurant.Name,
                    LogoUrl = member.Restaurant.LogoUrl,

                    Roles = member.MemberRoles
                        .Where(memberRole =>
                            memberRole.RestaurantId == member.RestaurantId &&
                            memberRole.Role.RestaurantId == member.RestaurantId &&
                            memberRole.Role.IsActive)
                        .OrderBy(memberRole => memberRole.Role.Name)
                        .ThenBy(memberRole => memberRole.RoleId)
                        .Select(memberRole => new AccountRestaurantRoleResponse
                        {
                            RoleId = memberRole.RoleId,
                            Name = memberRole.Role.Name,
                            Type = (int)memberRole.Role.Type
                        })
                        .DistinctBy(role => role.RoleId)
                        .ToList()
                })
                .ToList();
        }
    }
}
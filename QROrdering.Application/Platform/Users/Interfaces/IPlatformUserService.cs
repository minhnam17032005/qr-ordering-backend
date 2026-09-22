using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Platform.Users.DTOs.Requests;
using QROrdering.Application.Platform.Users.DTOs.Responses;

namespace QROrdering.Application.Platform.Users.Interfaces
{
    public interface IPlatformUserService
    {
        Task<PagedResponse<PlatformUserListResponse>>
            GetUsersAsync(
                PagedRequest request);

        Task<PlatformUserDetailResponse>
            GetUserByIdAsync(
                Guid userId);

        Task<PlatformUserDetailResponse>
        CreateUserAsync(
            CreatePlatformUserRequest request);

        Task<PlatformUserDetailResponse>
            UpdateUserAsync(
                Guid userId,
                UpdatePlatformUserRequest request);

        Task ActivateUserAsync(Guid userId);

        Task DeactivateUserAsync(Guid userId);
    }
}

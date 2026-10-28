using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Membership.DTOs.Requests;
using QROrdering.Application.Membership.DTOs.Responses;
using QROrdering.Infrastructure.Membership.DTOs.Requests;
using QROrdering.Infrastructure.Membership.DTOs.Responses;

namespace QROrdering.Application.Membership.Interfaces
{
    public interface IRestaurantMembershipService
    {
        Task<PagedResponse<RestaurantMemberListResponse>>
            GetMembersAsync(
                Guid restaurantId,
                PagedRequest request);

        Task<RestaurantMemberDetailResponse>
            GetMemberByIdAsync(
                Guid restaurantId,
                Guid memberId);

        Task<UserLookupResponse> LookupUserAsync(
                Guid restaurantId,
                UserLookupRequest request);

        Task<AddMemberResponse> AddMemberAsync(
            Guid restaurantId,
            AddMemberRequest request);

        Task<RegisterMemberResponse> RegisterMemberAsync(
            Guid restaurantId,
            RegisterMemberRequest request);

        Task<UpdateMemberRolesResponse> UpdateMemberRolesAsync(
            Guid restaurantId,
            Guid memberId,
        UpdateMemberRolesRequest request);

        Task<ActivateMemberResponse> ActivateMemberAsync(
            Guid restaurantId,
            Guid memberId);

        Task<DeactivateMemberResponse> DeactivateMemberAsync(
            Guid restaurantId,
            Guid memberId);

        Task<List<RestaurantRoleResponse>> GetRolesAsync(Guid restaurantId);

        Task<List<AccountRestaurantResponse>> GetMyRestaurantsAsync();
    }
}
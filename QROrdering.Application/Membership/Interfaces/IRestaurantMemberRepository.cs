using QROrdering.Domain.Entities.Membership;

namespace QROrdering.Application.Membership.Interfaces
{
    public interface IRestaurantMemberRepository
    {
        Task AddAsync(RestaurantMember restaurantMember);

        // Kiểm tra người dùng có phải Admin
        // đang hoạt động của nhà hàng hay không
        Task<bool> IsActiveAdminAsync(
            Guid userId,
            Guid restaurantId);

        // Đếm số thành viên của nhà hàng
        Task<int> CountByRestaurantAsync(
            Guid restaurantId);

        // Lấy danh sách thành viên có phân trang
        Task<List<RestaurantMember>> GetPagedByRestaurantAsync(
            Guid restaurantId,
            int page,
            int pageSize);

        Task<RestaurantMember?> GetByIdInRestaurantAsync(
            Guid restaurantId,
            Guid memberId);

        Task<RestaurantMember?> GetByUserIdAndRestaurantIdAsync(
            Guid userId,
            Guid restaurantId);

        Task<int> CountActiveAdminsAsync(Guid restaurantId);

        Task<List<RestaurantMember>> GetActiveMembershipsWithDetailsByUserIdAsync(
            Guid userId);
    }
}
using QROrdering.Domain.Entities.Membership;

namespace QROrdering.Application.Membership.Interfaces
{
    public interface IRestaurantMemberRepository
    {
        Task AddAsync(
            RestaurantMember restaurantMember);
    }
}
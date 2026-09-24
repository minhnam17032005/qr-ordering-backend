using QROrdering.Application.Membership.Interfaces;
using QROrdering.Domain.Entities.Membership;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Membership
{
    public class RestaurantMemberRepository
        : IRestaurantMemberRepository
    {
        private readonly QROrderingDbContext _context;

        public RestaurantMemberRepository(
            QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            RestaurantMember restaurantMember)
        {
            await _context.RestaurantMembers
                .AddAsync(restaurantMember);
        }
    }
}
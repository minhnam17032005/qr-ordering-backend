using Microsoft.EntityFrameworkCore;
using QROrdering.Application.Membership.Interfaces;
using QROrdering.Domain.Entities.Membership;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Membership
{
    public class MemberRoleRepository : IMemberRoleRepository
    {
        private readonly QROrderingDbContext _context;

        public MemberRoleRepository(QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MemberRole memberRole)
        {
            await _context.MemberRoles.AddAsync(memberRole);
        }

        public async Task<List<MemberRole>> GetByMemberIdAsync(
            Guid restaurantId,
            Guid memberId)
        {
            return await _context.MemberRoles
                .Include(x => x.Role)
                .Where(x =>
                    x.RestaurantId == restaurantId &&
                    x.RestaurantMemberId == memberId)
                .ToListAsync();
        }

        public void RemoveRange(IEnumerable<MemberRole> memberRoles)
        {
            _context.MemberRoles.RemoveRange(memberRoles);
        }
    }
}
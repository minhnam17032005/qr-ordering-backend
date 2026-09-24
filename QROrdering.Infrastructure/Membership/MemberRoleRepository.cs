using QROrdering.Application.Membership.Interfaces;
using QROrdering.Domain.Entities.Membership;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Membership
{
    public class MemberRoleRepository
        : IMemberRoleRepository
    {
        private readonly QROrderingDbContext _context;

        public MemberRoleRepository(
            QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            MemberRole memberRole)
        {
            await _context.MemberRoles
                .AddAsync(memberRole);
        }
    }
}
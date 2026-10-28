using QROrdering.Domain.Entities.Membership;

namespace QROrdering.Application.Membership.Interfaces
{
    public interface IMemberRoleRepository
    {
        Task AddAsync(MemberRole memberRole);

        Task<List<MemberRole>> GetByMemberIdAsync(
            Guid restaurantId,
            Guid memberId);

        void RemoveRange(
            IEnumerable<MemberRole> memberRoles);
    }
}
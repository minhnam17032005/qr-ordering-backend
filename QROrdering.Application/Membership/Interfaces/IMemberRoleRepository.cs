using QROrdering.Domain.Entities.Membership;

namespace QROrdering.Application.Membership.Interfaces
{
    public interface IMemberRoleRepository
    {
        Task AddAsync(
            MemberRole memberRole);
    }
}
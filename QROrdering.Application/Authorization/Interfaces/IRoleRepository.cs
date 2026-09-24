using QROrdering.Domain.Entities.Authorization;

namespace QROrdering.Application.Authorization.Interfaces
{
    public interface IRoleRepository
    {
        Task AddAsync(
            Role role);
    }
}
using QROrdering.Domain.Entities.Authorization;

namespace QROrdering.Application.Authorization.Interfaces
{
    public interface IRoleRepository
    {
        Task AddAsync(
            Role role);

        Task<List<Role>> GetByIdsInRestaurantAsync(
            Guid restaurantId,
            List<Guid> roleIds);

        Task<List<Role>> GetActiveByRestaurantIdAsync(
            Guid restaurantId);
    }
}
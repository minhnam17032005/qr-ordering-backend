using QROrdering.Domain.Entities.RestaurantManagement;

namespace QROrdering.Application.Platform.Restaurants.Interfaces
{
    public interface IPlatformRestaurantRepository
    {
        Task<int> CountAsync();

        Task<List<Restaurant>> GetPagedAsync(
            int page,
            int pageSize);

        Task<Restaurant?> GetByIdAsync(
            Guid restaurantId);

        Task AddAsync(
            Restaurant restaurant);
    }
}

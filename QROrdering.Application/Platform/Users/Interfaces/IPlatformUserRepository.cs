
using QROrdering.Domain.Entities.Identity;

namespace QROrdering.Application.Platform.Users.Interfaces
{
    public interface IPlatformUserRepository
    {
        Task<int> CountAsync();

        Task<List<User>> GetPagedAsync(
            int page,
            int pageSize);

        Task<User?> GetByIdWithRestaurantsAsync(
            Guid userId);

        Task<bool> ExistsByUsernameAsync(
       string username);

        Task<bool> ExistsByEmailAsync(
            string email);

        Task<bool> ExistsByUsernameExceptAsync(
            string username,
            Guid userId);

        Task<bool> ExistsByEmailExceptAsync(
            string email,
            Guid userId);

        Task AddAsync(User user);

        Task<User?> GetByIdForUpdateAsync(Guid userId);
    }
}

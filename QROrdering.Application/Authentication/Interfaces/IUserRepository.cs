using QROrdering.Domain.Entities.Identity;

namespace QROrdering.Application.Authentication.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsByUsernameAsync(string username);

        Task<bool> ExistsByEmailAsync(string email);

        Task AddAsync(User user);

        Task<User?> GetByIdentifierAsync(string identifier);

        Task<bool> ExistsByUsernameOrEmailOrPhoneAsync(
            string username,
            string email,
            string? phoneNumber);

        Task<User?> GetByIdAsync(Guid userId);

        Task<User?> GetByIdWithRestaurantMembershipsAsync(Guid userId);
    }
}

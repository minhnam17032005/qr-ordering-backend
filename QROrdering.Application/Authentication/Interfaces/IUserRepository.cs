using QROrdering.Domain.Entities.Identity;

namespace QROrdering.Application.Authentication.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsByUsernameAsync(string username);

        Task<bool> ExistsByEmailAsync(string email);

        Task AddAsync(User user);

        Task<User?> GetByEmailAsync(string email);

        Task<bool> ExistsByUsernameOrEmailAsync(string username,string email);
    }
}

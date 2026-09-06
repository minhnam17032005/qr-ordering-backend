namespace QROrdering.Application.Authentication.Interfaces
{
    public interface IUserSessionRepository
    {
        Task AddAsync(UserSession session);

        Task<UserSession?> GetByRefreshTokenHashWithUserAsync(
        string refreshTokenHash);
    }
}

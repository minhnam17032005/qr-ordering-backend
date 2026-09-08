namespace QROrdering.Application.Authentication.Interfaces
{
    public interface IUserSessionRepository
    {
        Task AddAsync(UserSession session);

        Task<UserSession?> GetByRefreshTokenHashWithUserAsync(
        string refreshTokenHash);

        Task<UserSession?> GetBySessionIdAsync(Guid sessionId);

        Task<UserSession?> GetSessionWithUserAsync(Guid sessionId);
    }
}

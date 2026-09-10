namespace QROrdering.Application.Authentication.Interfaces
{
    public interface IUserSessionRepository
    {
        Task AddAsync(UserSession session);

        Task<UserSession?> GetByRefreshTokenHashWithUserAsync(
        string refreshTokenHash);

        Task<UserSession?> GetBySessionIdAsync(Guid sessionId);

        Task<UserSession?> GetSessionWithUserAsync(Guid sessionId);

        /// Get all active sessions of a user.
        Task<List<UserSession>> GetActiveByUserIdAsync(Guid userId);

        Task<List<UserSession>> GetByUserIdAsync(Guid userId);

        Task<UserSession?> GetByIdAsync(Guid sessionId);

        Task<int> CountByUserIdAsync(Guid userId);

        Task<List<UserSession>> GetPagedByUserIdAsync(
            Guid userId,
            int page,
            int pageSize);
    }
}

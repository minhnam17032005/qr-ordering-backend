namespace QROrdering.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}

using QROrdering.Application.Common.Interfaces;

namespace QROrdering.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly QROrderingDbContext _context;

        public UnitOfWork(
            QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

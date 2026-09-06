using QROrdering.Application.Common.Interfaces;

namespace QROrdering.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        // DbContext dùng để quản lý transaction và lưu thay đổi
        private readonly QROrderingDbContext _context;

        public UnitOfWork(
            QROrderingDbContext context)
        {
            _context = context;
        }

        // Commit toàn bộ thay đổi vào database
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

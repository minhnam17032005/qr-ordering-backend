using QROrdering.Application.Common.Interfaces;

using Microsoft.EntityFrameworkCore.Storage;

namespace QROrdering.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        // DbContext dùng để quản lý transaction và lưu thay đổi
        private readonly QROrderingDbContext _context;

        private IDbContextTransaction? _transaction;

        public UnitOfWork(
            QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction =
                await _context.Database
                    .BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
            {
                return;
            }

            await _transaction.CommitAsync();

            await _transaction.DisposeAsync();

            _transaction = null;
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                return;
            }

            await _transaction.RollbackAsync();

            await _transaction.DisposeAsync();

            _transaction = null;
        }

        // Commit toàn bộ thay đổi vào database
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}


using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Exceptions;

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

        // Lưu thay đổi và chuyển lỗi unique constraint thành ConflictException.
        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is SqlException sqlException &&
                (sqlException.Number == 2601 ||
                 sqlException.Number == 2627))
            {
                throw new ConflictException(
                    "Dữ liệu đã tồn tại hoặc bị trùng.");
            }
        }
    }
}


using QROrdering.Application.Authorization.Interfaces;
using QROrdering.Domain.Entities.Authorization;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Authorization
{
    public class RoleRepository
        : IRoleRepository
    {
        private readonly QROrderingDbContext _context;

        public RoleRepository(
            QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            Role role)
        {
            await _context.Roles
                .AddAsync(role);
        }
    }
}
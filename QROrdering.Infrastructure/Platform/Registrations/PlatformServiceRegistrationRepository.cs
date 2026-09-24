using Microsoft.EntityFrameworkCore;

using QROrdering.Application.Platform.Registrations.Interfaces;
using QROrdering.Domain.Entities.Platform;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Platform.Registrations
{
    public class PlatformServiceRegistrationRepository
        : IPlatformServiceRegistrationRepository
    {
        private readonly QROrderingDbContext _context;

        public PlatformServiceRegistrationRepository(
            QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountAsync()
        {
            return await _context.ServiceRegistrations
                .CountAsync();
        }

        public async Task<List<ServiceRegistration>>
            GetPagedAsync(
                int page,
                int pageSize)
        {
            return await _context.ServiceRegistrations
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<ServiceRegistration?>
            GetByIdWithDetailsAsync(
                Guid registrationId)
        {
            return await _context.ServiceRegistrations
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == registrationId);
        }

        public async Task<ServiceRegistration?>
            GetByIdForUpdateAsync(
                Guid registrationId)
        {
            return await _context.ServiceRegistrations
                .FirstOrDefaultAsync(
                    x => x.Id == registrationId);
        }

        public void Update(
            ServiceRegistration registration)
        {
            _context.ServiceRegistrations
                .Update(registration);
        }
    }
}
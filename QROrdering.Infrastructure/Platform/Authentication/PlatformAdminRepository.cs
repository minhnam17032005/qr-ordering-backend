using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QROrdering.Application.Platform.Authentication.Interfaces;
using QROrdering.Domain.Entities.Platform;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Platform.Authentication
{
    public class PlatformAdminRepository: IPlatformAdminRepository
    {
        private readonly QROrderingDbContext _context;

        public PlatformAdminRepository(
            QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task<PlatformAdmin?> GetByIdentifierAsync(
            string identifier)
        {
            return await _context.PlatformAdmins
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Username == identifier ||
                    x.Email == identifier);
        }

        public async Task<PlatformAdmin?> GetByIdAsync(
        Guid platformAdminId)
        {
            return await _context.PlatformAdmins
                .FirstOrDefaultAsync(
                    x => x.Id == platformAdminId);
        }

        public async Task<PlatformAdmin?> GetByEmailAsync(
            string email)
        {
            return await _context.PlatformAdmins
                .FirstOrDefaultAsync(
                    x => x.Email == email);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QROrdering.Domain.Entities.Platform;

namespace QROrdering.Application.Platform.Authentication.Interfaces
{
    public interface IPlatformAdminRepository
    {
        Task<PlatformAdmin?> GetByIdentifierAsync(
            string identifier);

        Task<PlatformAdmin?> GetByIdAsync(
            Guid platformAdminId);

        Task<PlatformAdmin?> GetByEmailAsync(
            string email);  
    }
}

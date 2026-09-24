using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QROrdering.Domain.Entities.Identity;
using QROrdering.Domain.Entities.Platform;

namespace QROrdering.Application.Platform.Registrations.Interfaces
{
    public interface IPlatformServiceRegistrationRepository
    {
        Task<int> CountAsync();

        Task<List<ServiceRegistration>>
            GetPagedAsync(
                int page,
                int pageSize);

        Task<ServiceRegistration?>
            GetByIdWithDetailsAsync(
                Guid registrationId);

        Task<ServiceRegistration?>
            GetByIdForUpdateAsync(
                Guid registrationId);

        void Update(
            ServiceRegistration registration);
    }
}

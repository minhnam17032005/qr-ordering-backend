using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Platform.Registrations.DTOs.Responses;

namespace QROrdering.Application.Platform.Registrations.Interfaces
{
    public interface IPlatformServiceRegistrationService
    {
        Task<PagedResponse<PlatformServiceRegistrationListResponse>>
            GetRegistrationsAsync(
                PagedRequest request);

        Task<PlatformServiceRegistrationDetailResponse>
            GetRegistrationByIdAsync(
                Guid registrationId);

        Task ApproveRegistrationAsync(
        Guid registrationId);

        Task RejectRegistrationAsync(
        Guid registrationId);

    }
}

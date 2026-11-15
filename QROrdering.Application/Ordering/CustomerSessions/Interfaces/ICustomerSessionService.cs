using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Common.Responses;
using QROrdering.Application.Ordering.CustomerSessions.DTOs.Requests;
using QROrdering.Application.Ordering.CustomerSessions.DTOs.Responses;

namespace QROrdering.Application.Ordering.CustomerSessions.Interfaces
{
    public interface ICustomerSessionService
    {
        Task<CustomerSessionCreatedResponse> ScanTableAsync(
            ScanTableRequest request);

        Task<CustomerSessionDetailResponse> GetCurrentSessionAsync(
            string sessionToken);

        Task<PagedResponse<CustomerSessionListResponse>> GetSessionsAsync(
            Guid restaurantId,
            PagedRequest request);

        Task<CustomerSessionDetailResponse> GetSessionByIdAsync(
            Guid restaurantId,
            Guid sessionId);
    }
}
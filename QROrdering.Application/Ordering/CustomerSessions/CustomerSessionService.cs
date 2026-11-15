using System.Security.Cryptography;
using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Exceptions;
using QROrdering.Application.Ordering.CustomerSessions.DTOs.Requests;
using QROrdering.Application.Ordering.CustomerSessions.DTOs.Responses;
using QROrdering.Application.Ordering.CustomerSessions.Interfaces;
using QROrdering.Domain.Entities.Ordering;
using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.CustomerSessions
{
    public class CustomerSessionService : ICustomerSessionService
    {
        private readonly ICustomerSessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHashService _hashService;

        public CustomerSessionService(
            ICustomerSessionRepository sessionRepository,
            IUnitOfWork unitOfWork,
            IHashService hashService)
        {
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
            _hashService = hashService;
        }

        public async Task<CustomerSessionCreatedResponse> ScanTableAsync(
            ScanTableRequest request)
        {
            if (!await _sessionRepository.RestaurantExistsAsync(
                    request.RestaurantId))
            {
                throw new NotFoundException("Nhà hàng không tồn tại.");
            }

            if (!await _sessionRepository.RestaurantIsActiveAsync(
                    request.RestaurantId))
            {
                throw new BadRequestException("Nhà hàng hiện không hoạt động.");
            }

            var table = await _sessionRepository.GetTableForScanAsync(
                request.RestaurantId,
                request.TableId);

            if (table == null)
            {
                throw new NotFoundException("Bàn không tồn tại.");
            }

            if (!table.IsActive)
            {
                throw new BadRequestException("Bàn hiện không hoạt động.");
            }

            if (table.Status != TableStatus.Available)
            {
                throw new ConflictException("Bàn hiện không ở trạng thái sẵn sàng.");
            }

            if (await _sessionRepository.HasActiveSessionAsync(
                    request.RestaurantId,
                    request.TableId))
            {
                throw new ConflictException("Bàn đã có phiên khách hàng đang hoạt động.");
            }

            var sessionToken = Convert.ToHexString(
                RandomNumberGenerator.GetBytes(32));

            var now = DateTime.UtcNow;

            var session = new CustomerSession
            {
                RestaurantId = request.RestaurantId,
                TableId = request.TableId,
                SessionTokenHash = _hashService.Hash(sessionToken),
                Status = CustomerSessionStatus.Active,
                StartedAt = now
            };

            table.Status = TableStatus.Occupied;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _sessionRepository.AddAsync(session);

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new CustomerSessionCreatedResponse
                {
                    Id = session.Id,
                    RestaurantId = session.RestaurantId,
                    TableId = session.TableId,
                    TableNumber = table.TableNumber,
                    SessionToken = sessionToken,
                    Status = session.Status,
                    StartedAt = session.StartedAt
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<CustomerSessionDetailResponse> GetCurrentSessionAsync(
            string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(sessionToken))
            {
                throw new BadRequestException("SessionToken không hợp lệ.");
            }

            var tokenHash = _hashService.Hash(sessionToken);

            var session = await _sessionRepository.GetByTokenHashAsync(
                tokenHash);

            if (session == null)
            {
                throw new NotFoundException(
                    "Phiên khách hàng không tồn tại hoặc đã kết thúc.");
            }

            return session;
        }

        public async Task<PagedResponse<CustomerSessionListResponse>> GetSessionsAsync(
            Guid restaurantId,
            PagedRequest request)
        {
            if (!await _sessionRepository.RestaurantExistsAsync(restaurantId))
            {
                throw new NotFoundException("Nhà hàng không tồn tại.");
            }

            var totalCount = await _sessionRepository.CountByRestaurantAsync(
                restaurantId);

            var items = await _sessionRepository.GetPagedByRestaurantAsync(
                restaurantId,
                request);

            return new PagedResponse<CustomerSessionListResponse>
            {
                Items = items,
                Pagination = new PaginationMeta
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalItems = totalCount,
                    TotalPages = (int)Math.Ceiling(
            (double)totalCount / request.PageSize)
                }
            };
        }

        public async Task<CustomerSessionDetailResponse> GetSessionByIdAsync(
            Guid restaurantId,
            Guid sessionId)
        {
            if (!await _sessionRepository.RestaurantExistsAsync(restaurantId))
            {
                throw new NotFoundException("Nhà hàng không tồn tại.");
            }

            var session = await _sessionRepository.GetByIdInRestaurantAsync(
                restaurantId,
                sessionId);

            if (session == null)
            {
                throw new NotFoundException("Phiên khách hàng không tồn tại.");
            }

            return session;
        }
    }
}
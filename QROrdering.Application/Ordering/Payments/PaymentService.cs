using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Exceptions;
using QROrdering.Application.Ordering.CustomerSessions.Interfaces;
using QROrdering.Application.Ordering.Payments.DTOs.Requests;
using QROrdering.Application.Ordering.Payments.DTOs.Responses;
using QROrdering.Application.Ordering.Payments.Interfaces;
using QROrdering.Domain.Entities.Ordering;
using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICustomerSessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(
            IPaymentRepository paymentRepository,
            ICustomerSessionRepository sessionRepository,
            IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
        }

        // ============================================================
        // CREATE PAYMENT
        // ============================================================

        public async Task<PaymentResponse> CreatePaymentAsync(
            Guid restaurantId,
            Guid customerSessionId,
            PaymentCreateRequest request)
        {
            await ValidateRestaurantAsync(restaurantId);
            ValidatePaymentMethod(request.PaymentMethod);

            if (customerSessionId == Guid.Empty)
            {
                throw new BadRequestException(
                    "CustomerSessionId không hợp lệ.");
            }

            Guid paymentId;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Khóa CustomerSession trước khi kiểm tra và tính tiền.
                var session = await _sessionRepository.GetByIdForUpdateAsync(
                    restaurantId,
                    customerSessionId);

                if (session == null ||
                    session.Status != CustomerSessionStatus.Active)
                {
                    throw new BadRequestException(
                        "Phiên khách hàng không còn hoạt động.");
                }

                // Mỗi session chỉ được có một Payment Pending.
                var hasPendingPayment =
                    await _paymentRepository.HasPendingBySessionAsync(
                        restaurantId,
                        session.Id);

                if (hasPendingPayment)
                {
                    throw new ConflictException(
                        "Phiên khách hàng đã có giao dịch đang chờ thanh toán.");
                }

                // Tính số tiền phải thanh toán từ các món chưa bị hủy.
                var amount = await _paymentRepository
                    .CalculatePayableAmountAsync(
                        restaurantId,
                        session.Id);

                if (amount <= 0)
                {
                    throw new BadRequestException(
                        "Phiên khách hàng không có món hợp lệ để thanh toán.");
                }

                var now = DateTime.UtcNow;

                var payment = new Payment
                {
                    RestaurantId = restaurantId,
                    CustomerSessionId = session.Id,
                    PaymentMethod = request.PaymentMethod,
                    Status = PaymentStatus.Pending,
                    Amount = amount,
                    CreatedAt = now
                };

                await _paymentRepository.AddAsync(payment);

                paymentId = payment.Id;

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            var response = await _paymentRepository.GetByIdInRestaurantAsync(
                restaurantId,
                paymentId);

            if (response == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy giao dịch sau khi tạo.");
            }

            return response;
        }

        // ============================================================
        // GET PAYMENTS
        // ============================================================

        public async Task<PagedResponse<PaymentListResponse>> GetPaymentsAsync(
            Guid restaurantId,
            PaymentStatus? status,
            PagedRequest request)
        {
            await ValidateRestaurantAsync(restaurantId);

            var totalItems = await _paymentRepository.CountByRestaurantAsync(
                restaurantId,
                status);

            var items = await _paymentRepository.GetPagedByRestaurantAsync(
                restaurantId,
                status,
                request);

            return new PagedResponse<PaymentListResponse>
            {
                Items = items,
                Pagination = new PaginationMeta
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling(
                        (double)totalItems / request.PageSize)
                }
            };
        }

        // ============================================================
        // GET PAYMENT BY ID
        // ============================================================

        public async Task<PaymentResponse> GetPaymentByIdAsync(
            Guid restaurantId,
            Guid paymentId)
        {
            await ValidateRestaurantAsync(restaurantId);

            if (paymentId == Guid.Empty)
            {
                throw new BadRequestException(
                    "PaymentId không hợp lệ.");
            }

            var payment = await _paymentRepository.GetByIdInRestaurantAsync(
                restaurantId,
                paymentId);

            if (payment == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy giao dịch.");
            }

            return payment;
        }

        // ============================================================
        // CONFIRM PAYMENT
        // ============================================================

        public async Task<PaymentResponse> ConfirmPaymentAsync(
            Guid restaurantId,
            Guid paymentId,
            PaymentConfirmRequest request)
        {
            await ValidateRestaurantAsync(restaurantId);

            if (paymentId == Guid.Empty)
            {
                throw new BadRequestException(
                    "PaymentId không hợp lệ.");
            }

            // Lấy SessionId trước transaction để xác định bản ghi cần khóa.
            var sessionId = await _paymentRepository
                .GetSessionIdByPaymentIdAsync(
                    restaurantId,
                    paymentId);

            if (!sessionId.HasValue)
            {
                throw new NotFoundException(
                    "Không tìm thấy giao dịch.");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Khóa CustomerSession trước khi xác nhận Payment.
                var session = await _sessionRepository.GetByIdForUpdateAsync(
                    restaurantId,
                    sessionId.Value);

                if (session == null ||
                    session.Status != CustomerSessionStatus.Active)
                {
                    throw new BadRequestException(
                        "Phiên khách hàng không còn hoạt động.");
                }

                var payment = await _paymentRepository.GetEntityByIdAsync(
                    restaurantId,
                    paymentId);

                if (payment == null)
                {
                    throw new NotFoundException(
                        "Không tìm thấy giao dịch.");
                }

                if (payment.CustomerSessionId != session.Id)
                {
                    throw new BadRequestException(
                        "Giao dịch không thuộc phiên khách hàng hợp lệ.");
                }

                if (payment.Status != PaymentStatus.Pending)
                {
                    throw new BadRequestException(
                        "Chỉ có thể xác nhận giao dịch đang chờ thanh toán.");
                }

                var now = DateTime.UtcNow;

                // Xác nhận Payment.
                payment.Status = PaymentStatus.Completed;
                payment.PaidAt = now;
                payment.TransactionCode =
                    NormalizeOptionalString(request.TransactionCode);
                payment.UpdatedAt = now;

                // Hoàn tất CustomerSession.
                session.Status = CustomerSessionStatus.Completed;
                session.EndedAt = now;
                session.UpdatedAt = now;

                // Giải phóng bàn sau khi thanh toán thành công.
                var table = await _sessionRepository.GetTableForScanAsync(
                    restaurantId,
                    session.TableId);

                if (table == null)
                {
                    throw new NotFoundException(
                        "Không tìm thấy bàn thuộc phiên khách hàng.");
                }

                table.Status = TableStatus.Available;
                table.UpdatedAt = now;

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            var response = await _paymentRepository.GetByIdInRestaurantAsync(
                restaurantId,
                paymentId);

            if (response == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy giao dịch sau khi xác nhận.");
            }

            return response;
        }

        // ============================================================
        // RETRY PAYMENT
        // ============================================================

        public async Task<PaymentResponse> RetryPaymentAsync(
            Guid restaurantId,
            Guid paymentId,
            PaymentCreateRequest request)
        {
            await ValidateRestaurantAsync(restaurantId);
            ValidatePaymentMethod(request.PaymentMethod);

            if (paymentId == Guid.Empty)
            {
                throw new BadRequestException(
                    "PaymentId không hợp lệ.");
            }

            // Xác định session của Payment cũ.
            var sessionId = await _paymentRepository
                .GetSessionIdByPaymentIdAsync(
                    restaurantId,
                    paymentId);

            if (!sessionId.HasValue)
            {
                throw new NotFoundException(
                    "Không tìm thấy giao dịch.");
            }

            Guid newPaymentId;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Tất cả thao tác liên quan session phải dùng cùng khóa.
                var session = await _sessionRepository.GetByIdForUpdateAsync(
                    restaurantId,
                    sessionId.Value);

                if (session == null ||
                    session.Status != CustomerSessionStatus.Active)
                {
                    throw new BadRequestException(
                        "Phiên khách hàng không còn hoạt động.");
                }

                var oldPayment = await _paymentRepository.GetEntityByIdAsync(
                    restaurantId,
                    paymentId);

                if (oldPayment == null)
                {
                    throw new NotFoundException(
                        "Không tìm thấy giao dịch.");
                }

                if (oldPayment.CustomerSessionId != session.Id)
                {
                    throw new BadRequestException(
                        "Giao dịch không thuộc phiên khách hàng hợp lệ.");
                }

                if (oldPayment.Status != PaymentStatus.Failed)
                {
                    throw new BadRequestException(
                        "Chỉ có thể tạo lại giao dịch từ Payment Failed.");
                }

                var hasPendingPayment =
                    await _paymentRepository.HasPendingBySessionAsync(
                        restaurantId,
                        session.Id);

                if (hasPendingPayment)
                {
                    throw new ConflictException(
                        "Phiên khách hàng đã có giao dịch đang chờ thanh toán.");
                }

                // Tính lại số tiền vì dữ liệu đơn hàng có thể đã thay đổi.
                var amount = await _paymentRepository
                    .CalculatePayableAmountAsync(
                        restaurantId,
                        session.Id);

                if (amount <= 0)
                {
                    throw new BadRequestException(
                        "Phiên khách hàng không có món hợp lệ để thanh toán.");
                }

                var now = DateTime.UtcNow;

                var newPayment = new Payment
                {
                    RestaurantId = restaurantId,
                    CustomerSessionId = session.Id,
                    PaymentMethod = request.PaymentMethod,
                    Status = PaymentStatus.Pending,
                    Amount = amount,
                    CreatedAt = now
                };

                await _paymentRepository.AddAsync(newPayment);

                newPaymentId = newPayment.Id;

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            var response = await _paymentRepository.GetByIdInRestaurantAsync(
                restaurantId,
                newPaymentId);

            if (response == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy giao dịch mới sau khi tạo.");
            }

            return response;
        }

        // ============================================================
        // PRIVATE METHODS
        // ============================================================

        private async Task ValidateRestaurantAsync(Guid restaurantId)
        {
            if (restaurantId == Guid.Empty)
            {
                throw new BadRequestException(
                    "RestaurantId không hợp lệ.");
            }

            var exists = await _paymentRepository.RestaurantExistsAsync(
                restaurantId);

            if (!exists)
            {
                throw new NotFoundException(
                    "Không tìm thấy nhà hàng.");
            }
        }

        private static void ValidatePaymentMethod(PaymentMethod method)
        {
            if (!Enum.IsDefined(typeof(PaymentMethod), method))
            {
                throw new BadRequestException(
                    "Phương thức thanh toán không hợp lệ.");
            }
        }

        private static string? NormalizeOptionalString(string? value)
        {
            var normalized = value?.Trim();

            return string.IsNullOrWhiteSpace(normalized)
                ? null
                : normalized;
        }
    }
}
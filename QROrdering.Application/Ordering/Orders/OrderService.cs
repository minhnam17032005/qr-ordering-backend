using System.Security.Cryptography;
using QROrdering.Application.Common.Interfaces;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Exceptions;
using QROrdering.Application.Ordering.CustomerSessions.Interfaces;
using QROrdering.Application.Ordering.Orders.DTOs.Requests;
using QROrdering.Application.Ordering.Orders.DTOs.Responses;
using QROrdering.Application.Ordering.Orders.Interfaces;
using QROrdering.Domain.Entities.Ordering;
using QROrdering.Application.Ordering.CustomerSessions.DTOs.Responses;
using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerSessionRepository _sessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHashService _hashService;

        public OrderService(
            IOrderRepository orderRepository,
            ICustomerSessionRepository sessionRepository,
            IUnitOfWork unitOfWork,
            IHashService hashService)
        {
            _orderRepository = orderRepository;
            _sessionRepository = sessionRepository;
            _unitOfWork = unitOfWork;
            _hashService = hashService;
        }

        // ============================================================
        // CUSTOMER APIs
        // ============================================================

        public async Task<OrderDetailResponse> CreateOrderAsync(
            OrderCreateRequest request,
            string sessionToken)
        {
            var session = await GetActiveSessionAsync(sessionToken);

            if (request.Items == null || request.Items.Count == 0)
            {
                throw new BadRequestException(
                    "Đơn hàng phải có ít nhất một sản phẩm.");
            }

            if (request.Items.Any(x =>
                x.ProductId == Guid.Empty || x.Quantity <= 0))
            {
                throw new BadRequestException(
                    "Thông tin sản phẩm hoặc số lượng không hợp lệ.");
            }

            var duplicateProduct = request.Items
                .GroupBy(x => x.ProductId)
                .Any(x => x.Count() > 1);

            if (duplicateProduct)
            {
                throw new BadRequestException(
                    "Sản phẩm bị trùng trong danh sách đặt hàng.");
            }

            var productIds = request.Items
                .Select(x => x.ProductId)
                .ToList();

            var products = await _orderRepository
                .GetActiveProductsForOrderAsync(
                    session.RestaurantId,
                    productIds);

            if (products.Count != productIds.Count)
            {
                throw new BadRequestException(
                    "Một hoặc nhiều sản phẩm không tồn tại, không thuộc nhà hàng hoặc đã ngừng kinh doanh.");
            }

            var productDictionary = products.ToDictionary(x => x.Id);

            var now = DateTime.UtcNow;

            var order = new Order
            {
                RestaurantId = session.RestaurantId,
                CustomerSessionId = session.Id,
                OrderCode = GenerateOrderCode(),
                Status = OrderStatus.Pending,
                Note = NormalizeOptionalString(request.Note),
                TotalAmount = 0,
                CreatedAt = now
            };

            foreach (var itemRequest in request.Items)
            {
                var product = productDictionary[itemRequest.ProductId];

                var orderItem = new OrderItem
                {
                    RestaurantId = session.RestaurantId,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = product.Price,
                    Note = NormalizeOptionalString(itemRequest.Note),
                    Status = OrderItemStatus.Pending,
                    CreatedAt = now
                };

                order.OrderItems.Add(orderItem);

                order.TotalAmount +=
                    product.Price * itemRequest.Quantity;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _orderRepository.AddAsync(order);

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            var response = await _orderRepository.GetRestaurantOrderByIdAsync(
                session.RestaurantId,
                order.Id);

            if (response == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy đơn hàng sau khi tạo.");
            }

            return response;
        }

        public async Task<PagedResponse<OrderListResponse>>
            GetCustomerOrdersAsync(
                string sessionToken,
                PagedRequest request)
        {
            var session = await GetActiveSessionAsync(sessionToken);

            var totalItems = await _orderRepository.CountCustomerOrdersAsync(
                session.Id);

            var items = await _orderRepository.GetCustomerOrdersPagedAsync(
                session.Id,
                request);

            return CreatePagedResponse(items, request, totalItems);
        }

        public async Task<OrderDetailResponse> GetCustomerOrderByIdAsync(
            string sessionToken,
            Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                throw new BadRequestException("OrderId không hợp lệ.");
            }

            var session = await GetActiveSessionAsync(sessionToken);

            var order = await _orderRepository.GetCustomerOrderByIdAsync(
                session.Id,
                orderId);

            if (order == null)
            {
                throw new NotFoundException("Không tìm thấy đơn hàng.");
            }

            return order;
        }

        // ============================================================
        // RESTAURANT APIs
        // ============================================================

        public async Task<PagedResponse<OrderListResponse>>
            GetRestaurantOrdersAsync(
                Guid restaurantId,
                OrderStatus? status,
                PagedRequest request)
        {
            await ValidateRestaurantAsync(restaurantId);

            var totalItems = await _orderRepository
                .CountRestaurantOrdersAsync(restaurantId, status);

            var items = await _orderRepository
                .GetRestaurantOrdersPagedAsync(
                    restaurantId,
                    status,
                    request);

            return CreatePagedResponse(items, request, totalItems);
        }

        public async Task<OrderDetailResponse> GetRestaurantOrderByIdAsync(
            Guid restaurantId,
            Guid orderId)
        {
            await ValidateRestaurantAsync(restaurantId);

            if (orderId == Guid.Empty)
            {
                throw new BadRequestException("OrderId không hợp lệ.");
            }

            var order = await _orderRepository.GetRestaurantOrderByIdAsync(
                restaurantId,
                orderId);

            if (order == null)
            {
                throw new NotFoundException("Không tìm thấy đơn hàng.");
            }

            return order;
        }

        public async Task<OrderStatusResponse> UpdateOrderStatusAsync(
            Guid restaurantId,
            Guid orderId,
            OrderStatusUpdateRequest request)
        {
            await ValidateRestaurantAsync(restaurantId);

            if (orderId == Guid.Empty)
            {
                throw new BadRequestException("OrderId không hợp lệ.");
            }

            if (request.Status != OrderStatus.Cancelled)
            {
                throw new BadRequestException(
                    "Chỉ cho phép hủy toàn bộ đơn hàng bằng API này.");
            }

            var order = await _orderRepository.GetOrderEntityByIdAsync(
                restaurantId,
                orderId);

            if (order == null)
            {
                throw new NotFoundException("Không tìm thấy đơn hàng.");
            }

            if (order.Status == OrderStatus.Completed ||
                order.Status == OrderStatus.Cancelled)
            {
                throw new BadRequestException(
                    "Đơn hàng đã kết thúc, không thể thay đổi trạng thái.");
            }

            if (order.OrderItems.Any(x =>
                x.Status == OrderItemStatus.Completed))
            {
                throw new BadRequestException(
                    "Không thể hủy đơn hàng đã có món hoàn thành.");
            }

            var now = DateTime.UtcNow;

            foreach (var item in order.OrderItems)
            {
                if (item.Status == OrderItemStatus.Pending ||
                    item.Status == OrderItemStatus.Preparing)
                {
                    item.Status = OrderItemStatus.Cancelled;
                    item.UpdatedAt = now;
                }
            }

            order.Status = CalculateOrderStatus(order.OrderItems);
            order.UpdatedAt = now;

            await _unitOfWork.SaveChangesAsync();

            return new OrderStatusResponse
            {
                Id = order.Id,
                Status = order.Status
            };
        }

        public async Task<OrderItemStatusResponse>
            UpdateOrderItemStatusAsync(
                Guid restaurantId,
                Guid orderItemId,
                OrderItemStatusUpdateRequest request)
        {
            await ValidateRestaurantAsync(restaurantId);

            if (orderItemId == Guid.Empty)
            {
                throw new BadRequestException(
                    "OrderItemId không hợp lệ.");
            }

            var orderItem = await _orderRepository
                .GetOrderItemWithOrderAsync(
                    restaurantId,
                    orderItemId);

            if (orderItem == null)
            {
                throw new NotFoundException(
                    "Không tìm thấy sản phẩm trong đơn hàng.");
            }

            if (!IsValidItemStatusTransition(
                orderItem.Status,
                request.Status))
            {
                throw new BadRequestException(
                    "Không thể chuyển trạng thái món ăn theo yêu cầu.");
            }

            var now = DateTime.UtcNow;

            orderItem.Status = request.Status;
            orderItem.UpdatedAt = now;

            orderItem.Order.Status = CalculateOrderStatus(
                orderItem.Order.OrderItems);

            orderItem.Order.UpdatedAt = now;

            await _unitOfWork.SaveChangesAsync();

            return new OrderItemStatusResponse
            {
                Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                Status = orderItem.Status,
                OrderStatus = orderItem.Order.Status
            };
        }

        // ============================================================
        // PRIVATE METHODS
        // ============================================================

        private async Task<CustomerSessionDetailResponse> GetActiveSessionAsync(string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(sessionToken))
            {
                throw new BadRequestException(
                    "SessionToken không hợp lệ.");
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

        private async Task ValidateRestaurantAsync(Guid restaurantId)
        {
            if (restaurantId == Guid.Empty)
            {
                throw new BadRequestException(
                    "RestaurantId không hợp lệ.");
            }

            var exists = await _orderRepository.RestaurantExistsAsync(
                restaurantId);

            if (!exists)
            {
                throw new NotFoundException(
                    "Không tìm thấy nhà hàng.");
            }
        }

        private static bool IsValidItemStatusTransition(
            OrderItemStatus currentStatus,
            OrderItemStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                (OrderItemStatus.Pending,
                    OrderItemStatus.Preparing) => true,

                (OrderItemStatus.Pending,
                    OrderItemStatus.Cancelled) => true,

                (OrderItemStatus.Preparing,
                    OrderItemStatus.Completed) => true,

                (OrderItemStatus.Preparing,
                    OrderItemStatus.Cancelled) => true,

                _ => false
            };
        }

        private static OrderStatus CalculateOrderStatus(
            IEnumerable<OrderItem> items)
        {
            var itemList = items.ToList();

            if (itemList.Count == 0)
            {
                return OrderStatus.Pending;
            }

            if (itemList.All(x =>
                x.Status == OrderItemStatus.Cancelled))
            {
                return OrderStatus.Cancelled;
            }

            if (itemList.All(x =>
                x.Status == OrderItemStatus.Completed ||
                x.Status == OrderItemStatus.Cancelled))
            {
                return OrderStatus.Completed;
            }

            if (itemList.All(x =>
                x.Status == OrderItemStatus.Pending))
            {
                return OrderStatus.Pending;
            }

            return OrderStatus.Preparing;
        }

        private static string GenerateOrderCode()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"
                .Substring(0, 21)
                .ToUpperInvariant();
        }

        private static string? NormalizeOptionalString(string? value)
        {
            var normalized = value?.Trim();

            return string.IsNullOrWhiteSpace(normalized)
                ? null
                : normalized;
        }

        private static PagedResponse<T> CreatePagedResponse<T>(
            List<T> items,
            PagedRequest request,
            int totalItems)
        {
            return new PagedResponse<T>
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
    }
}
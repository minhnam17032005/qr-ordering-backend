using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Ordering.Orders.DTOs.Responses;
using QROrdering.Domain.Entities.Ordering;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.Orders.Interfaces
{
    public interface IOrderRepository
    {
        Task<bool> RestaurantExistsAsync(Guid restaurantId);

        Task<int> CountCustomerOrdersAsync(Guid customerSessionId);

        Task<List<OrderListResponse>> GetCustomerOrdersPagedAsync(
            Guid customerSessionId,
            PagedRequest request);

        Task<OrderDetailResponse?> GetCustomerOrderByIdAsync(
            Guid customerSessionId,
            Guid orderId);

        Task<int> CountRestaurantOrdersAsync(
            Guid restaurantId,
            OrderStatus? status);

        Task<List<OrderListResponse>> GetRestaurantOrdersPagedAsync(
            Guid restaurantId,
            OrderStatus? status,
            PagedRequest request);

        Task<OrderDetailResponse?> GetRestaurantOrderByIdAsync(
            Guid restaurantId,
            Guid orderId);

        Task<List<Product>> GetActiveProductsForOrderAsync(
            Guid restaurantId,
            List<Guid> productIds);

        Task<Order?> GetOrderEntityByIdAsync(
            Guid restaurantId,
            Guid orderId);

        Task<OrderItem?> GetOrderItemWithOrderAsync(
            Guid restaurantId,
            Guid orderItemId);

        Task AddAsync(Order order);

        Task<Guid?> GetOrderSessionIdAsync(
            Guid restaurantId,
            Guid orderId);

        Task<Guid?> GetOrderItemSessionIdAsync(
            Guid restaurantId,
            Guid orderItemId);
    }
}
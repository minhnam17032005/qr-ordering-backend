using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Ordering.Orders.DTOs.Requests;
using QROrdering.Application.Ordering.Orders.DTOs.Responses;
using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.Orders.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDetailResponse> CreateOrderAsync(
            OrderCreateRequest request,
            string sessionToken);

        Task<PagedResponse<OrderListResponse>> GetCustomerOrdersAsync(
            string sessionToken,
            PagedRequest request);

        Task<OrderDetailResponse> GetCustomerOrderByIdAsync(
            string sessionToken,
            Guid orderId);

        Task<PagedResponse<OrderListResponse>> GetRestaurantOrdersAsync(
            Guid restaurantId,
            OrderStatus? status,
            PagedRequest request);

        Task<OrderDetailResponse> GetRestaurantOrderByIdAsync(
            Guid restaurantId,
            Guid orderId);

        Task<OrderStatusResponse> UpdateOrderStatusAsync(
            Guid restaurantId,
            Guid orderId,
            OrderStatusUpdateRequest request);

        Task<OrderItemStatusResponse> UpdateOrderItemStatusAsync(
            Guid restaurantId,
            Guid orderItemId,
            OrderItemStatusUpdateRequest request);
    }
}
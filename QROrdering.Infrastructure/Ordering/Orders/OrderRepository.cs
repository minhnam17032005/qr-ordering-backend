using Microsoft.EntityFrameworkCore;
using QROrdering.Application.Common.Pagination;
using QROrdering.Application.Ordering.Orders.DTOs.Responses;
using QROrdering.Application.Ordering.Orders.Interfaces;
using QROrdering.Domain.Entities.Ordering;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Domain.Enums;
using QROrdering.Infrastructure.Persistence;

namespace QROrdering.Infrastructure.Ordering.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly QROrderingDbContext _context;

        public OrderRepository(QROrderingDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RestaurantExistsAsync(Guid restaurantId)
        {
            return await _context.Restaurants
                .AnyAsync(x => x.Id == restaurantId);
        }

        public async Task<int> CountCustomerOrdersAsync(
            Guid customerSessionId)
        {
            return await _context.Orders
                .CountAsync(x => x.CustomerSessionId == customerSessionId);
        }

        public async Task<List<OrderListResponse>> GetCustomerOrdersPagedAsync(
            Guid customerSessionId,
            PagedRequest request)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Where(x => x.CustomerSessionId == customerSessionId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenBy(x => x.Id);

            var skip = (long)(request.Page - 1) * request.PageSize;

            if (skip > int.MaxValue)
            {
                return [];
            }

            return await query
                .Skip((int)skip)
                .Take(request.PageSize)
                .Select(x => new OrderListResponse
                {
                    Id = x.Id,
                    OrderCode = x.OrderCode,
                    TableNumber = x.CustomerSession.RestaurantTable.TableNumber,
                    Status = x.Status,
                    TotalAmount = x.TotalAmount,
                    ItemCount = x.OrderItems.Count,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<OrderDetailResponse?> GetCustomerOrderByIdAsync(
            Guid customerSessionId,
            Guid orderId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Where(x =>
                    x.CustomerSessionId == customerSessionId &&
                    x.Id == orderId)
                .Select(x => new OrderDetailResponse
                {
                    Id = x.Id,
                    RestaurantId = x.RestaurantId,
                    CustomerSessionId = x.CustomerSessionId,
                    OrderCode = x.OrderCode,
                    TableNumber = x.CustomerSession.RestaurantTable.TableNumber,
                    Status = x.Status,
                    Note = x.Note,
                    TotalAmount = x.TotalAmount,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,

                    Items = x.OrderItems
                        .OrderBy(i => i.CreatedAt)
                        .Select(i => new OrderItemResponse
                        {
                            Id = i.Id,
                            ProductId = i.ProductId,
                            ProductName = i.ProductName,
                            Quantity = i.Quantity,
                            UnitPrice = i.UnitPrice,
                            TotalPrice = i.UnitPrice * i.Quantity,
                            Note = i.Note,
                            Status = i.Status
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CountRestaurantOrdersAsync(
            Guid restaurantId,
            OrderStatus? status)
        {
            var query = _context.Orders
                .Where(x => x.RestaurantId == restaurantId);

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            return await query.CountAsync();
        }

        public async Task<List<OrderListResponse>> GetRestaurantOrdersPagedAsync(
            Guid restaurantId,
            OrderStatus? status,
            PagedRequest request)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Where(x => x.RestaurantId == restaurantId);

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            var skip = (long)(request.Page - 1) * request.PageSize;

            if (skip > int.MaxValue)
            {
                return [];
            }

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .Skip((int)skip)
                .Take(request.PageSize)
                .Select(x => new OrderListResponse
                {
                    Id = x.Id,
                    OrderCode = x.OrderCode,
                    TableNumber = x.CustomerSession.RestaurantTable.TableNumber,
                    Status = x.Status,
                    TotalAmount = x.TotalAmount,
                    ItemCount = x.OrderItems.Count,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<OrderDetailResponse?> GetRestaurantOrderByIdAsync(
            Guid restaurantId,
            Guid orderId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Where(x =>
                    x.RestaurantId == restaurantId &&
                    x.Id == orderId)
                .Select(x => new OrderDetailResponse
                {
                    Id = x.Id,
                    RestaurantId = x.RestaurantId,
                    CustomerSessionId = x.CustomerSessionId,
                    OrderCode = x.OrderCode,
                    TableNumber = x.CustomerSession.RestaurantTable.TableNumber,
                    Status = x.Status,
                    Note = x.Note,
                    TotalAmount = x.TotalAmount,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,

                    Items = x.OrderItems
                        .OrderBy(i => i.CreatedAt)
                        .Select(i => new OrderItemResponse
                        {
                            Id = i.Id,
                            ProductId = i.ProductId,
                            ProductName = i.ProductName,
                            Quantity = i.Quantity,
                            UnitPrice = i.UnitPrice,
                            TotalPrice = i.UnitPrice * i.Quantity,
                            Note = i.Note,
                            Status = i.Status
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<Product>> GetActiveProductsForOrderAsync(
            Guid restaurantId,
            List<Guid> productIds)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(x =>
                    x.RestaurantId == restaurantId &&
                    x.IsActive &&
                    productIds.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<Order?> GetOrderEntityByIdAsync(
            Guid restaurantId,
            Guid orderId)
        {
            return await _context.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x =>
                    x.RestaurantId == restaurantId &&
                    x.Id == orderId);
        }

        public async Task<OrderItem?> GetOrderItemWithOrderAsync(
            Guid restaurantId,
            Guid orderItemId)
        {
            return await _context.OrderItems
                .Include(x => x.Order)
                    .ThenInclude(x => x.OrderItems)
                .FirstOrDefaultAsync(x =>
                    x.RestaurantId == restaurantId &&
                    x.Id == orderItemId);
        }

        public Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            return Task.CompletedTask;
        }

        public async Task<Guid?> GetOrderSessionIdAsync(
            Guid restaurantId,
            Guid orderId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Where(x =>
                    x.RestaurantId == restaurantId &&
                    x.Id == orderId)
                .Select(x => (Guid?)x.CustomerSessionId)
                .FirstOrDefaultAsync();
        }

        public async Task<Guid?> GetOrderItemSessionIdAsync(
            Guid restaurantId,
            Guid orderItemId)
        {
            return await _context.OrderItems
                .AsNoTracking()
                .Where(x =>
                    x.RestaurantId == restaurantId &&
                    x.Id == orderItemId)
                .Select(x => (Guid?)x.Order.CustomerSessionId)
                .FirstOrDefaultAsync();
        }
    }
}
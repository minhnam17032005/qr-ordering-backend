using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Entities.Authorization;
using QROrdering.Domain.Entities.History;
using QROrdering.Domain.Entities.Membership;
using QROrdering.Domain.Entities.Ordering;
using QROrdering.Domain.Entities.Platform;

namespace QROrdering.Domain.Entities.RestaurantManagement
{
    public class Restaurant : BaseEntity
    {
        public string Name { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Description { get; set; }

        public string? LogoUrl { get; set; }

        public bool IsActive { get; set; }

        // Navigation Properties
        public ICollection<RestaurantTable> RestaurantTables { get; set; }
    = new List<RestaurantTable>();

        public ICollection<Category> Categories { get; set; }
            = new List<Category>();

        public ICollection<Product> Products { get; set; }
            = new List<Product>();

        public ICollection<RestaurantMember> RestaurantMembers { get; set; }
            = new List<RestaurantMember>();

        public ICollection<Role> Roles { get; set; }
            = new List<Role>();

        public ICollection<CustomerSession> CustomerSessions { get; set; }
            = new List<CustomerSession>();

        public ICollection<Order> Orders { get; set; }
            = new List<Order>();

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();

        public ICollection<RevenueDaily> RevenueDailies { get; set; }
            = new List<RevenueDaily>();

        public ICollection<OrderHistory> OrderHistories { get; set; }
            = new List<OrderHistory>();

        public ICollection<ServiceRegistration> ServiceRegistrations { get; set; }
            = new List<ServiceRegistration>();
    }
}

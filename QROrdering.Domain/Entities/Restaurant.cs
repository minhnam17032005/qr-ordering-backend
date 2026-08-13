using System.ComponentModel.DataAnnotations;

namespace QROrdering.Domain.Entities
{
    public class Restaurant : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = null!;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        [Url]
        public string? LogoUrl { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<RestaurantTable> RestaurantTables { get; set; }
            = new List<RestaurantTable>();

        public ICollection<Category> Categories { get; set; }
            = new List<Category>();

        public ICollection<Product> Products { get; set; }
            = new List<Product>();

        public ICollection<User> Users { get; set; }
            = new List<User>();

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
    }
}

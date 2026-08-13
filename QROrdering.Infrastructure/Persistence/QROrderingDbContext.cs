using Microsoft.EntityFrameworkCore;
using QROrdering.Domain.Entities;

namespace QROrdering.Infrastructure.Persistence
{
    public class QROrderingDbContext : DbContext
    {
        public QROrderingDbContext(DbContextOptions<QROrderingDbContext> options)
            : base(options)
        {
        }

        // =========================
        // Restaurant
        // =========================

        public DbSet<Restaurant> Restaurants { get; set; }

        public DbSet<RestaurantTable> RestaurantTables { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        // =========================
        // Ordering
        // =========================

        public DbSet<CustomerSession> CustomerSessions { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Payment> Payments { get; set; }

        // =========================
        // Order History
        // =========================

        public DbSet<OrderHistory> OrderHistories { get; set; }

        public DbSet<OrderItemHistory> OrderItemHistories { get; set; }

        // =========================
        // User & Authorization
        // =========================

        public DbSet<User> Users { get; set; }

        public DbSet<UserSession> UserSessions { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        // =========================
        // Notification
        // =========================

        public DbSet<Notification> Notifications { get; set; }

        // =========================
        // Revenue
        // =========================

        public DbSet<RevenueDaily> RevenueDailies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(QROrderingDbContext).Assembly);
        }
    }
}
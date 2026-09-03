using Microsoft.EntityFrameworkCore;
using QROrdering.Domain.Entities.Authorization;
using QROrdering.Domain.Entities.History;
using QROrdering.Domain.Entities.Identity;
using QROrdering.Domain.Entities.Membership;
using QROrdering.Domain.Entities.Ordering;
using QROrdering.Domain.Entities.Platform;
using QROrdering.Domain.Entities.RestaurantManagement;

namespace QROrdering.Infrastructure.Persistence
{
    public class QROrderingDbContext : DbContext
    {
        public QROrderingDbContext(DbContextOptions<QROrderingDbContext> options)
            : base(options)
        {
        }

        // ============================================================
        // IDENTITY
        // ============================================================

        public DbSet<User> Users => Set<User>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();


        // ============================================================
        // RESTAURANT MANAGEMENT
        // ============================================================

        public DbSet<Restaurant> Restaurants => Set<Restaurant>();
        public DbSet<RestaurantTable> RestaurantTables => Set<RestaurantTable>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<RevenueDaily> RevenueDailies => Set<RevenueDaily>();


        // ============================================================
        // MEMBERSHIP
        // ============================================================

        public DbSet<RestaurantMember> RestaurantMembers => Set<RestaurantMember>();
        public DbSet<MemberRole> MemberRoles => Set<MemberRole>();


        // ============================================================
        // AUTHORIZATION
        // ============================================================

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();


        // ============================================================
        // ORDERING
        // ============================================================

        public DbSet<CustomerSession> CustomerSessions => Set<CustomerSession>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<OrderHistory> OrderHistories => Set<OrderHistory>();
        public DbSet<OrderItemHistory> OrderItemHistories => Set<OrderItemHistory>();


        // ============================================================
        // PLATFORM
        // ============================================================

        public DbSet<PlatformAdmin> PlatformAdmins => Set<PlatformAdmin>();
        public DbSet<PlatformAdminSession> PlatformAdminSessions => Set<PlatformAdminSession>();
        public DbSet<ServiceRegistration> ServiceRegistrations => Set<ServiceRegistration>();


        // ============================================================
        // MODEL CONFIGURATION
        // ============================================================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================================================
            // APPLY ALL ENTITY CONFIGURATIONS
            // ========================================================

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(QROrderingDbContext).Assembly
            );
        }
    }
}
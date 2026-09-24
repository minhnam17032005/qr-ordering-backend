using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QROrdering.Domain.Entities.Platform;
using QROrdering.Domain.Enums;

namespace QROrdering.Infrastructure.Persistence.Configurations
{
    public class ServiceRegistrationConfiguration
        : IEntityTypeConfiguration<ServiceRegistration>
    {
        public void Configure(EntityTypeBuilder<ServiceRegistration> builder)
        {
            // ============================================================
            // TABLE
            // ============================================================

            builder.ToTable("ServiceRegistrations");


            // ============================================================
            // BASE ENTITY
            // ============================================================

            builder.ConfigureBaseEntity();


            // ============================================================
            // PROPERTIES
            // ============================================================

            // ============================================================
            // PROPERTIES
            // ============================================================

            // Người đăng ký

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.ContactName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(20);


            // Thông tin nhà hàng đăng ký

            builder.Property(x => x.RestaurantName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.RestaurantAddress)
                .HasMaxLength(500);

            builder.Property(x => x.RestaurantPhoneNumber)
                .HasMaxLength(20);

            builder.Property(x => x.RestaurantEmail)
                .HasMaxLength(255);

            builder.Property(x => x.RestaurantDescription)
                .HasMaxLength(1000);

            builder.Property(x => x.RestaurantLogoUrl)
                .HasMaxLength(500);


            // Trạng thái đăng ký

            builder.Property(x => x.Status)
                .IsRequired()
                .HasDefaultValue(ServiceRegistrationStatus.Pending)
                .HasSentinel((ServiceRegistrationStatus)0);

            builder.Property(x => x.Note)
                .HasMaxLength(1000);


            // Người Platform xử lý

            builder.Property(x => x.ProcessedAt);


            // ============================================================
            // INDEXES
            // ============================================================
            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => x.Email);

            builder.HasIndex(x => x.Status);

            builder.HasIndex(x => x.ProcessedByPlatformAdminId);

            builder.HasIndex(x => x.RestaurantId);


            // ============================================================
            // RELATIONSHIPS
            // ============================================================

            // User 1 - N ServiceRegistration
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // PlatformAdmin 1 - N ServiceRegistration
            builder.HasOne(x => x.ProcessedByPlatformAdmin)
                .WithMany(x => x.ServiceRegistrations)
                .HasForeignKey(x => x.ProcessedByPlatformAdminId)
                .OnDelete(DeleteBehavior.NoAction);


            // Restaurant 1 - N ServiceRegistration
            builder.HasOne(x => x.Restaurant)
                .WithMany(x => x.ServiceRegistrations)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
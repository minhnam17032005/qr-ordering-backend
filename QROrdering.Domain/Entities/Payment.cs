using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public Guid OrderId { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [MaxLength(100)]
        public string? TransactionCode { get; set; }

        public DateTime? PaidAt { get; set; }

        // Navigation Properties

        public Order Order { get; set; } = null!;
    }
}

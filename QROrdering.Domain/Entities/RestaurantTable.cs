using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities
{
    public class RestaurantTable : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        [Range(1, int.MaxValue)]
        public int TableNumber { get; set; }

        [Required]
        [MaxLength(500)]
        public string QRCode { get; set; } = null!;

        [Required]
        public TableStatus Status { get; set; }

        public bool IsActive { get; set; } = true;


        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;

        public ICollection<CustomerSession> CustomerSessions { get; set; }
            = new List<CustomerSession>();
    }
}

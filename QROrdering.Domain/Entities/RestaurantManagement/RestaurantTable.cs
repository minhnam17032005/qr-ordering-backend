using System.ComponentModel.DataAnnotations;
using QROrdering.Domain.Entities.Ordering;
using QROrdering.Domain.Enums;

namespace QROrdering.Domain.Entities.RestaurantManagement
{
    public class RestaurantTable : BaseEntity
    {
        public Guid RestaurantId { get; set; }

        public int TableNumber { get; set; }

        public string QRCode { get; set; } = null!;

        public TableStatus Status { get; set; }

        public bool IsActive { get; set; } 


        // Navigation Properties

        public Restaurant Restaurant { get; set; } = null!;

        public ICollection<CustomerSession> CustomerSessions { get; set; }
            = new List<CustomerSession>();
    }
}

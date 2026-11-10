using QROrdering.Domain.Enums;

namespace QROrdering.Application.RestaurantManagement.Tables.DTOs.Responses
{
    public class TableDetailResponse
    {
        public Guid Id { get; set; }

        public Guid RestaurantId { get; set; }

        public int TableNumber { get; set; }

        public string QRCode { get; set; } = null!;

        public TableStatus Status { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
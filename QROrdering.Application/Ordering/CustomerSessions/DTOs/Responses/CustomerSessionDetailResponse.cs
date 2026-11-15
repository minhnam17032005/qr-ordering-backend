using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.CustomerSessions.DTOs.Responses
{
    public class CustomerSessionDetailResponse
    {
        public Guid Id { get; set; }

        public Guid RestaurantId { get; set; }

        public Guid TableId { get; set; }

        public int TableNumber { get; set; }

        public CustomerSessionStatus Status { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? EndedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
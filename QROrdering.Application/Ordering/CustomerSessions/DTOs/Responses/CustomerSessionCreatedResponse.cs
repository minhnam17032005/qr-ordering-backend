using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.CustomerSessions.DTOs.Responses
{
    public class CustomerSessionCreatedResponse
    {
        public Guid Id { get; set; }

        public Guid RestaurantId { get; set; }

        public Guid TableId { get; set; }

        public int TableNumber { get; set; }

        public string SessionToken { get; set; } = null!;

        public CustomerSessionStatus Status { get; set; }

        public DateTime StartedAt { get; set; }
    }
}
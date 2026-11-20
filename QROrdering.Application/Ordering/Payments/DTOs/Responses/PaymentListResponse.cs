using QROrdering.Domain.Enums;

namespace QROrdering.Application.Ordering.Payments.DTOs.Responses
{
    public class PaymentListResponse
    {
        public Guid Id { get; set; }

        public Guid CustomerSessionId { get; set; }

        public int TableNumber { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public PaymentStatus Status { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace QROrdering.Application.Ordering.Payments.DTOs.Requests
{
    public class PaymentConfirmRequest
    {
        [MaxLength(100)]
        public string? TransactionCode { get; set; }
    }
}
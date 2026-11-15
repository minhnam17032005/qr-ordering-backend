namespace QROrdering.Application.Ordering.CustomerSessions.DTOs.Requests
{
    public class ScanTableRequest
    {
        public Guid RestaurantId { get; set; }

        public Guid TableId { get; set; }
    }
}
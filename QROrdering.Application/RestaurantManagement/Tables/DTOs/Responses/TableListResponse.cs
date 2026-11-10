using QROrdering.Domain.Enums;

namespace QROrdering.Application.RestaurantManagement.Tables.DTOs.Responses
{
    public class TableListResponse
    {
        public Guid Id { get; set; }

        public int TableNumber { get; set; }

        public TableStatus Status { get; set; }

        public bool IsActive { get; set; }
    }
}
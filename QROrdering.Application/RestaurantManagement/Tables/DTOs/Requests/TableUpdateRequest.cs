using System.ComponentModel.DataAnnotations;

namespace QROrdering.Application.RestaurantManagement.Tables.DTOs.Requests
{
    public class TableUpdateRequest
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Số bàn phải lớn hơn 0.")]
        public int TableNumber { get; set; }
    }
}
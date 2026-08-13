namespace QROrdering.Domain.Enums
{
    public enum OrderItemStatus
    {
        Pending = 1,      // Món mới được đặt
        Confirmed = 2,    // Nhà hàng đã xác nhận món
        Preparing = 3,    // Món đang được chuẩn bị
        Completed = 4,    // Món đã hoàn thành
        Cancelled = 5     // Món bị hủy
    }
}

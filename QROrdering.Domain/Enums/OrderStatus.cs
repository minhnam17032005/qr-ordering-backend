namespace QROrdering.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1,      // Đơn mới tạo
        Confirmed = 2,    // Nhà hàng đã xác nhận
        Preparing = 3,    // Đang chuẩn bị món
        Completed = 4,    // Đã hoàn tất
        Cancelled = 5     // Đã hủy
    }
}

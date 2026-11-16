public enum OrderStatus
{
    Pending = 1,    // Đơn mới được gửi
    Preparing = 2,  // Nhà hàng đang xử lý đơn
    Completed = 3,  // Đơn đã hoàn tất
    Cancelled = 4   // Đơn đã bị hủy
}
namespace QROrdering.Domain.Enums
{
    public enum NotificationType
    {
        NewOrder = 1,        // Có đơn hàng mới
        OrderUpdated = 2,    // Đơn hàng được cập nhật
        Payment = 3,        // Có thay đổi liên quan đến thanh toán
        System = 4           // Thông báo hệ thống
    }
}

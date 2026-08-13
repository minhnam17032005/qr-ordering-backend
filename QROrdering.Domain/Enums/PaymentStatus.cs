namespace QROrdering.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending = 1,     // Chờ thanh toán
        Processing = 2,  // Đang xử lý
        Completed = 3,   // Thanh toán thành công
        Failed = 4      // Thanh toán thất bại
    }
}

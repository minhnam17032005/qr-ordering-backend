namespace QROrdering.Application.RestaurantManagement.Products.DTOs.Responses
{
    public class ProductDetailResponse
    {
        public Guid Id { get; set; }

        public Guid RestaurantId { get; set; }

        public Guid CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
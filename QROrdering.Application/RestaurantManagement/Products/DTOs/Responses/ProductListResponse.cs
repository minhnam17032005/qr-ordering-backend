namespace QROrdering.Application.RestaurantManagement.Products.DTOs.Responses
{
    public class ProductListResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public CategorySummaryResponse Category { get; set; } = null!;
    }
}
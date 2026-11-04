namespace QROrdering.Application.RestaurantManagement.Categories.DTOs.Responses
{
    public class CategoryDetailResponse
    {
        public Guid Id { get; set; }

        public Guid RestaurantId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
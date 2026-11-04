namespace QROrdering.Application.RestaurantManagement.Categories.DTOs.Requests
{
    public class CategoryCreateRequest
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}

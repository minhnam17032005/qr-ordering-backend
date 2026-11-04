namespace QROrdering.Application.RestaurantManagement.Categories.DTOs.Requests
{
    public class CategoryUpdateRequest
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace QROrdering.Application.RestaurantManagement.Products.DTOs.Requests
{
    public class ProductUpdateRequest
    {
        [Required]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự.")]
        public string Name { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        public string? Description { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999.99",
            ErrorMessage = "Giá sản phẩm không hợp lệ.")]
        public decimal Price { get; set; }

        [StringLength(500, ErrorMessage = "ImageUrl không được vượt quá 500 ký tự.")]
        public string? ImageUrl { get; set; }
    }
}
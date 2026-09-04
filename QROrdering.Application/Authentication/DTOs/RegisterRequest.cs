using System.ComponentModel.DataAnnotations;

namespace QROrdering.Application.Authentication.DTOs
{
    public class RegisterRequest
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = null!;
    }
}

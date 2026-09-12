using System.ComponentModel.DataAnnotations;

namespace QROrdering.Application.Authentication.DTOs.Requests
{
    public class LoginRequest
    {
        [Required]
        public string Identifier { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}

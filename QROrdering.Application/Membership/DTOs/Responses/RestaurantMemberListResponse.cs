
namespace QROrdering.Application.Membership.DTOs.Responses
{
    public class RestaurantMemberListResponse
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Username { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}

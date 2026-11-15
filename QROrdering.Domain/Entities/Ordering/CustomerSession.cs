using QROrdering.Domain;
using QROrdering.Domain.Entities.Ordering;
using QROrdering.Domain.Entities.RestaurantManagement;
using QROrdering.Domain.Enums;

public class CustomerSession : BaseEntity
{
    public Guid RestaurantId { get; set; }
    public Guid TableId { get; set; }

    public string SessionTokenHash { get; set; } = null!;

    public CustomerSessionStatus Status { get; set; }

    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    public Restaurant Restaurant { get; set; } = null!;
    public RestaurantTable RestaurantTable { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
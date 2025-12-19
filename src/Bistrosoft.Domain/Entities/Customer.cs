using Bistrosoft.Domain.ValueObjects;

namespace Bistrosoft.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Email Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}


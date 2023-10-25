using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace SaleApi.Models.Entity;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public string Filter { get; set; } = null!;

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
    public string? CustomerId { get; set; }
	
	public virtual Customer? Customer { get; set; }
    [JsonIgnore]

    public virtual Role Role { get; set; } = null!;
}

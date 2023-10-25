using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace SaleApi.Models.Entity;

public partial class Product
{
    public string ProductId { get; set; } = null!;

    public string? UserId { get; set; }

    public string? CategoryId { get; set; }

    public string? PromotionId { get; set; }

    public string ProductName { get; set; } = null!;

    public int? ProductPrice { get; set; }

    public string? ProductDes { get; set; }

    public string? ProductImage { get; set; }

    public bool IsNew { get; set; }

    public bool IsSale { get; set; }

    public bool IsPro { get; set; }

    public string Filter { get; set; } = null!;

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
	[JsonIgnore]
	public virtual Category? Category { get; set; }

	public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
	[JsonIgnore]
	public virtual Promotion? Promotion { get; set; }
	[JsonIgnore]
	public virtual User? User { get; set; }

}

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SaleApi.Models.Entity;

public partial class Promotion
{
    public string PromotionId { get; set; } = null!;
    public string PromotionName { get; set; } = null!;
    public string? PromotionDes { get; set; }

    public string? UserId { get; set; }

    public int? Discount { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Filter { get; set; } = null!;

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    [JsonIgnore]
    public virtual User? User { get; set; }
}

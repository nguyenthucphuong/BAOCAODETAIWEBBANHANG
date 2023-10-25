using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SaleApi.Models.Entity;
public partial class Order
{
    public string OrderId { get; set; } = null!;
    public string OrderName { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string PaymentId { get; set; } = null!;
    public string PromotionId { get; set; } = null!;
    public int DiscountCode { get; set; }
    public string OrderStatusId { get; set; } = null!;
    public long Total { get; set; }
    public DateTime OrderDatetime { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string? DeliveryTimeSlot { get; set; }
    public string? GhiChu { get; set; }
    public bool IsActive { get; set; }
    public string Filter { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual OrderStatus OrderStatus { get; set; } = null!;
    public virtual Payment Payment { get; set; } = null!;
    public virtual Promotion Promotion { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

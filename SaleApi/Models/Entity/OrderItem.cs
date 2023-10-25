using System;
using System.Collections.Generic;

namespace SaleApi.Models.Entity;

public partial class OrderItem
{
    public string OrderItemId { get; set; } = null!;
    public string OrderItemName { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public string? OrderId { get; set; }

    public int Quantity { get; set; }

    public int Price { get; set; }

    public DateTime OrderItemDatetime { get; set; }

    public bool IsActive { get; set; }

    public string Filter { get; set; } = null!;

    public virtual Order? Order { get; set; }

    public virtual Product Product { get; set; } = null!;
}

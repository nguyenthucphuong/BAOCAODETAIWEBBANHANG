using System;
using System.Collections.Generic;

namespace SaleApi.Models.Entity;

public partial class OrderStatus
{
    public string OrderStatusId { get; set; } = null!;

    public string OrderStatusName { get; set; } = null!;

    public bool IsActive { get; set; }

    public string Filter { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

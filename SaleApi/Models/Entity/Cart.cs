using System;
using System.Collections.Generic;

namespace SaleApi.Models.Entity;

public partial class Cart
{
    public string CartId { get; set; } = null!;
    public string CartName { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public string? ProductName { get; set; }
    public string? ProductImage { get; set; }
    public int ProductPrice { get; set; }
    public int Quantity { get; set; }
    public string UserId { get; set; } = null!;
    public string Filter { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

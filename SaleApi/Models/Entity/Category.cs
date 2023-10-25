using System;
using System.Collections.Generic;

namespace SaleApi.Models.Entity;

public partial class Category
{
    public string CategoryId { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public string Filter { get; set; } = null!;

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

}

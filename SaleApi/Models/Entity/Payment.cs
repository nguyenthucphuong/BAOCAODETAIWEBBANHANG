using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata;
using SaleApi.Controllers;

namespace SaleApi.Models.Entity;
    public partial class Payment 
{
    public string PaymentId { get; set; } = null!;

    public string PaymentName { get; set; } = null!;

    public bool IsActive { get; set; }

    public string Filter { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

}


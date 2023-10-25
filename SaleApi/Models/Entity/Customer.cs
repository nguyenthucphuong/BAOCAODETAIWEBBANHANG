using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SaleApi.Models.Entity;

public partial class Customer
{
    public string CustomerName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? Ward { get; set; } = null!;
    public string? District { get; set; } = null!;
    public string? City { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Filter { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CustomerId { get; set; } = null!;
    [JsonIgnore]
    public virtual User? User { get; set; }
}

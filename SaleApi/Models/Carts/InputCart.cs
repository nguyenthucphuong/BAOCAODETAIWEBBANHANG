﻿using SaleApi.Models.Entity;

namespace SaleApi.Models.Carts
{
    public class InputCart
    {
        public string? CartId { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}

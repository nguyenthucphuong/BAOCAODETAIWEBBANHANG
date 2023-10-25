using Microsoft.AspNetCore.Http.HttpResults;
using SaleApi.Models.Entity;
using SaleApi.Models.Orders;
using System.Net;
using Common.Utilities;

namespace SaleApi.Models.Extended
{
    public partial class OrderEx : Order
    {
        public string PaymentName { get; set; } = string.Empty;
        public string TextColorPaymentName { get; set; } = string.Empty;
        public string BackgroundColorPaymentName { get; set; } = string.Empty;
        public string PromotionName { get; set; } = string.Empty;
        public string OrderStatusName { get; set; } = string.Empty;
        public string TextColorOrderStatusName { get; set; } = string.Empty;
        public string BackgroundColorOrderStatusName { get; set; } = string.Empty;
        public int DiscountValue { get; set; }

        public OrderEx()
        {
            OrderId = Guid.NewGuid().ToString();
            OrderDatetime = DateTime.Now;
            IsActive = true;
        }
        public OrderEx(Order order)
        {
            OrderId = order.OrderId;
            OrderName = order.OrderName;
            UserId = order.UserId;
            PaymentId = order.PaymentId;

            PaymentName = string.Empty;
            TextColorPaymentName = string.Empty;
            BackgroundColorPaymentName = string.Empty;

            PromotionId = order.PromotionId;
            PromotionName = string.Empty;
            DiscountCode = order.DiscountCode;
            DiscountValue = 0;
            OrderStatusId = order.OrderStatusId;

            OrderStatusName = string.Empty;
            TextColorPaymentName= string.Empty;
            BackgroundColorOrderStatusName = string.Empty;

            Total = order.Total;
            OrderDatetime = order.OrderDatetime;
            GhiChu = order.GhiChu;
            DeliveryDate = order.DeliveryDate;
            DeliveryTimeSlot = order.DeliveryTimeSlot;
            IsActive = order.IsActive;
            Filter = order.Filter;
            OrderItems = order.OrderItems;
            OrderStatus = order.OrderStatus;
            Payment = order.Payment;
            Promotion = order.Promotion;
            User = order.User;
        }

		public OrderEx(InputOrder input)
		{
			OrderId = Guid.NewGuid().ToString();
			DiscountCode = input.DiscountCode ?? 0;
			Total = input.Total;
			DeliveryDate = input.DeliveryDate ?? DateTime.Now;
			DeliveryTimeSlot = input.DeliveryTimeSlot;
			GhiChu = input.GhiChu ?? "";
			OrderDatetime = DateTime.Now;
			OrderName = "HD" + OrderDatetime.ToString("yyyyMMddHHmmss") + new Random().Next(1, 1000);
			IsActive = true;
			Filter = Utility.Filter(OrderName, DiscountCode, GhiChu);
		}
		
	}
}

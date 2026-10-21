using System;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public class Coupon
    {
        public string Code { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal MinOrderValue { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
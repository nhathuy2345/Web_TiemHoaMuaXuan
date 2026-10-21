using System;
using System.Collections.Generic;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public static class CouponData
    {
        // Danh sách mã giảm giá lưu trên RAM (tự mất khi tắt web)
        public static List<Coupon> Coupons = new List<Coupon>
        {
            // Tạo sẵn 2 mã mẫu để test
            new Coupon { Code = "SPRING20", DiscountAmount = 20000, MinOrderValue = 200000, ExpiryDate = DateTime.Now.AddYears(1), IsActive = true },
            new Coupon { Code = "GIAM50K", DiscountAmount = 50000, MinOrderValue = 500000, ExpiryDate = DateTime.Now.AddYears(1), IsActive = true }
        };
    }
}
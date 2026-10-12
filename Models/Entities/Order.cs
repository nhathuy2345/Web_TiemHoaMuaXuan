using System;
using System.Collections.Generic;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    /// <summary>
    /// Đơn hàng
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }       // DH1001
        public string UserEmail { get; set; }        // Email người đặt
        public string FullName { get; set; }         // Người nhận
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }    // COD hoặc Bank
        public string Status { get; set; }            // Chờ xử lý, Đang giao, Đã giao, Đã hủy
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // Danh sách sản phẩm trong đơn
        public List<OrderItem> Items { get; set; }
    }

    /// <summary>
    /// Chi tiết 1 sản phẩm trong đơn hàng
    /// </summary>
    public class OrderItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Thành tiền — tự tính
        public decimal Total => Price * Quantity;
    }
}
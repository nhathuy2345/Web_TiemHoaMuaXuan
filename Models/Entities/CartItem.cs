using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Thuộc tính tính tổng tiền tự động
        public decimal TotalPrice
        {
            get { return Price * Quantity; }
        }
    }
}
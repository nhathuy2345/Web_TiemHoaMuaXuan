using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public class ProductStatItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int TotalSold { get; set; }
    }
}
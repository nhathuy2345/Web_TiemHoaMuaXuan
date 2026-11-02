using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string OrderCode { get; set; }

        [StringLength(200)]
        public string UserEmail { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, StringLength(20)]
        public string Phone { get; set; }

        [Required, StringLength(500)]
        public string Address { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Chờ xử lý";

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal")]
        public decimal TotalAmount { get; set; }

        // Navigation — 1 Order có nhiều OrderItem
        public virtual ICollection<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        [StringLength(200)]
        public string ProductName { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        // Navigation — Thuộc về 1 Order
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        // Computed — không lưu vào DB
        [NotMapped]
        public decimal Total => Price * Quantity;
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required, StringLength(200)]
        public string UserEmail { get; set; }

        [Required, StringLength(100)]
        public string UserName { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } // Số sao (1-5)

        [Required, StringLength(1000)]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property (nếu cần)
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
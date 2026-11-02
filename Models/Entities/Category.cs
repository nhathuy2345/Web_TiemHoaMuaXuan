using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation — 1 Category có nhiều Product
        public virtual ICollection<Product> Products { get; set; }
    }
}
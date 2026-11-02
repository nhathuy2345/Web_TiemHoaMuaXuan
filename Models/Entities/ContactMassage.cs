using System;
using System.ComponentModel.DataAnnotations;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public class ContactMessage
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [Required, StringLength(2000)]
        public string Message { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;

        public bool IsRead { get; set; }
    }
}
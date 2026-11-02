using System;
using System.ComponentModel.DataAnnotations;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserEmail { get; set; } // Email người chat

        [Required]
        public string Message { get; set; } // Nội dung

        public DateTime SentAt { get; set; } = DateTime.Now;

        public bool IsAdmin { get; set; } // true: Admin gửi, false: Khách gửi
    }
}
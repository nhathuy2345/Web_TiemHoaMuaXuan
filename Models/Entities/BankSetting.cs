using System.ComponentModel.DataAnnotations;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public class BankSetting
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên ngân hàng")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tài khoản")]
        public string AccountNumber { get; set; }

        public string AccountHolder { get; set; }

        public string QRImageUrl { get; set; }

        // ⭐ THÊM DÒNG NÀY VÀO MODEL
        public string SePayApiKey { get; set; }
    }
}
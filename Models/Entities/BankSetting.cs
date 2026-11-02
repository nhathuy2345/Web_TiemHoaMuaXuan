using System.ComponentModel.DataAnnotations;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    /// <summary>
    /// Cấu hình ngân hàng — lưu DB
    /// Chỉ có 1 row duy nhất (Id = 1)
    /// </summary>
    public class BankSetting
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string BankName { get; set; }

        [StringLength(50)]
        public string AccountNumber { get; set; }

        [StringLength(200)]
        public string AccountHolder { get; set; }

        [StringLength(500)]
        public string QRImageUrl { get; set; }
    }
}
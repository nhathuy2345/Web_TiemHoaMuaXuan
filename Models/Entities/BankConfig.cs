namespace Web_TiemHoaMuaXuan.Models.Entities
{
    /// <summary>
    /// Cấu hình ngân hàng cho QR thanh toán
    /// Lưu trong memory, admin có thể sửa
    /// </summary>
    public static class BankSettings
    {
        public static string BankName { get; set; } = "Techcombank";
        public static string AccountNumber { get; set; } = "10000865070207";
        public static string AccountHolder { get; set; } = "TIEM HOA MUA XUAN";
        public static string QRImageUrl { get; set; } = "";  // URL ảnh QR (nếu muốn upload riêng)
    }
}
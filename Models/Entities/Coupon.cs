using System;
using System.ComponentModel.DataAnnotations; // Bắt buộc phải using thư viện này

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public class Coupon
    {
        // 1. Bắt buộc nhập, không được để trống
        // 2. Nếu chuyển sang dùng Database (Entity Framework), [Key] sẽ biến Code thành Khóa chính
        [Key] 
        [Required(ErrorMessage = "Mã Code không được để trống")]
        [StringLength(50, ErrorMessage = "Mã Code không được vượt quá 50 ký tự")]
        public string Code { get; set; }

        // 1. Bắt buộc nhập
        // 2. Range: Ép kiểu dữ liệu phải là số dương lớn hơn 1000đ (tránh nhập số âm)
        [Required(ErrorMessage = "Vui lòng nhập số tiền giảm")]
        [Range(1000, double.MaxValue, ErrorMessage = "Số tiền giảm phải từ 1,000 VNĐ trở lên")]
        public decimal DiscountAmount { get; set; }

        // 1. Bắt buộc nhập
        // 2. Range: Đơn tối thiểu không được âm
        [Required(ErrorMessage = "Vui lòng nhập giá trị đơn hàng tối thiểu")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn tối thiểu không được là số âm")]
        public decimal MinOrderValue { get; set; }

        // 1. Bắt buộc nhập
        // 2. DataType: Định dạng kiểu dữ liệu là Ngày tháng (hiển thị DatePicker)
        [Required(ErrorMessage = "Vui lòng chọn ngày hết hạn")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
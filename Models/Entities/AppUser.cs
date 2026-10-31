using System.ComponentModel.DataAnnotations;

namespace Web_TiemHoaMuaXuan.Models.Entities
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } // Thêm trường mật khẩu

        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string FullName { get; set; }

        public string Phone { get; set; }
        public string Address { get; set; }

        public bool IsAdmin { get; set; } = false;

        // Thêm trạng thái để Admin có thể khóa tài khoản
        public bool IsActive { get; set; } = true;
    }
}
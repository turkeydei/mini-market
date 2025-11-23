using System.ComponentModel.DataAnnotations;

namespace WebShop.Models
{
    public class ProfileUpdateViewModel
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20)]
        public string? DienThoai { get; set; }

        [StringLength(200)]
        public string? DiaChi { get; set; }

        public int? GioiTinh { get; set; } // 0 = Nam, 1 = Nữ

        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }
    }
}

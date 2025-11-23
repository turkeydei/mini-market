using System.ComponentModel.DataAnnotations;

namespace WebShop.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        public string MatKhauCu { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự")]
        public string MatKhauMoi { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string XacNhanMatKhau { get; set; } = default!;
    }
}

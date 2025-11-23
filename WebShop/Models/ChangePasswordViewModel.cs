//using System.ComponentModel.DataAnnotations;

//namespace WebShop.Models
//{
//    public class ChangePasswordViewModel
//    {
//        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
//        public string MatKhauCu { get; set; } = default!;

//        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
//        [MinLength(6, ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự")]
//        public string MatKhauMoi { get; set; } = default!;

//        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
//        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không khớp")]
//        public string XacNhanMatKhau { get; set; } = default!;
//    }
//}
using System.ComponentModel.DataAnnotations;

namespace WebShop.Models
{
    // 1. Kế thừa IValidatableObject
    public class ChangePasswordViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        public string MatKhauCu { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự")]
        public string MatKhauMoi { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string XacNhanMatKhau { get; set; } = default!;

        // 2. Triển khai hàm Validate
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Kiểm tra nếu Mật khẩu mới trùng với Mật khẩu cũ
            if (MatKhauMoi == MatKhauCu)
            {
                // Trả về lỗi, gắn lỗi này vào trường "MatKhauMoi" để nó hiện đỏ dưới ô nhập liệu đó
                yield return new ValidationResult(
                    "Mật khẩu mới không được trùng với mật khẩu cũ.",
                    new[] { nameof(MatKhauMoi) }
                );
            }
        }
    }
}
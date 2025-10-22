using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("KhuyenMai")]
    public class KhuyenMai
    {
        [Key]
        public int MaKM { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Tên Chương Trình Khuyến Mãi")]
        public string? TenChuongTrinh { get; set; }

        [Required]
        [Display(Name = "Ngày Bắt Đầu")]
        public DateTime NgayBatDau { get; set; }

        [Required]
        [Display(Name = "Ngày Kết Thúc")]
        public DateTime NgayKetThuc { get; set; }

        [Required]
        [Display(Name = "Giảm Giá (%)")]
        [Range(0, 100, ErrorMessage = "Giảm giá phải nằm trong khoảng từ 0 đến 100.")]
        public float GiamGia { get; set; }


        [StringLength(255)]
        [Display(Name = "Điều Kiện")]
        public string? DieuKien { get; set; }

        // Navigation Properties
        public  ICollection<ChiTietKhuyenMai>? ChiTietKhuyenMais { get; set; }
    }
}

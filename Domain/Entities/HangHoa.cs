using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("HangHoa")]
    public class HangHoa
    {
        [Key]
        public int MaHH { get; set; }

        [Required, StringLength(255)]
        [Display(Name = "Tên Hàng Hóa")]
        public string TenHH { get; set; } = null!;

        [Required]
        public int MaLoai { get; set; }
        public Loai Loai { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        [StringLength(255)]
        [Display(Name = "Hình Ảnh (relative path)")]
        public string? Hinh { get; set; }

        [Display(Name = "Ngày Sản Xuất")]
        public DateTime? NgaySX { get; set; }  // để optional cho linh hoạt

        [Column(TypeName = "decimal(5,2)")]
        public decimal GiamGia { get; set; } = 0m;

        public int SoLanXem { get; set; } = 0;

        public string? MoTa { get; set; }

        // Navigation
        public ICollection<ChiTietHD> ChiTietHDs { get; set; } = new List<ChiTietHD>();
    }
}

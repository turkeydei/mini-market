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

        [Required]
        [StringLength(255)]
        [Display(Name = "Tên Hàng Hóa")]
        public string? TenHH { get; set; }

        public int MaLoai { get; set; }
        public Loai? Loai { get; set; }

        public float? DonGia { get; set; }

        [StringLength(255)]
        [Display(Name = "Hình Ảnh")]
        public string? Hinh { get; set; }

        [Required]
        [Display(Name = "Ngày Sản Xuất")]
        public DateTime NgaySX { get; set; }

        [Required]
        public float GiamGia { get; set; }

        [Required]
        public int SoLanXem { get; set; }

        public string? MoTa { get; set; }

        // Navigation Properties
        public ICollection<ChiTietHD>? ChiTietHDs { get; set; }
        public ICollection<TonKho>? TonKhos { get; set; }
    }
}

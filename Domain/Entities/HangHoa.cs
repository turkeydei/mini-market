using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required]
        public int MaLoai { get; set; }
        public Loai? Loai { get; set; }

        [Required]
        public decimal DonGia { get; set; }

        [StringLength(255)]
        [Display(Name = "Hình Ảnh")]
        public string? Hinh { get; set; }

        [Required]
        public int SoLuongTon { get; set; } = 0;

        public decimal GiamGia { get; set; } = 0;

        public int SoLanXem { get; set; } = 0;

        public string? MoTa { get; set; }

        // Navigation Properties
        public ICollection<ChiTietHD>? ChiTietHDs { get; set; }
    }
}

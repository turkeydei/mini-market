using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("LichSuGiaoDich")]
    public class LichSuGiaoDich
    {
        [Key]
        public int MaGiaoDich { get; set; }

        [Required]
        public int MaUser { get; set; }  // Sử dụng MaUser thay cho MaKH
        public User? User { get; set; }  // Điều hướng tới User thay cho KhachHang

        [Required]
        public int MaHD { get; set; }
        public HoaDon? HoaDon { get; set; }

        [Required]
        [Display(Name = "Ngày Giao Dịch")]
        public DateTime NgayGiaoDich { get; set; }

        [Required]
        public float TongTien { get; set; }
    }
}

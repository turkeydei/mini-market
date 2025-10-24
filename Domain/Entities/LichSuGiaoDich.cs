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
        public int MaHD { get; set; }
        public HoaDon HoaDon { get; set; } = null!;

        [Required, StringLength(20)]
        public string Provider { get; set; } = "VNPAY";

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal SoTien { get; set; }

        [Required, StringLength(20)]
        public string TrangThai { get; set; } = "Pending"; // Pending, Success, Failed

        [StringLength(100)]
        public string? MaGiaoDichVNPAY { get; set; } // Từ VNPAY trả về (vnp_TxnRef)

        [StringLength(255)]
        public string? MoTa { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
        public DateTime? NgayCapNhat { get; set; }
    }
}

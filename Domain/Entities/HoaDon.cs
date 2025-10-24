using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("HoaDon")]
public class HoaDon
{
    [Key]
    public int MaHD { get; set; }

    [Required]
    public int MaUser { get; set; }
    public User User { get; set; } = null!;

    [Required]
    [Display(Name = "Ngày Đặt")]
    public DateTime NgayDat { get; set; } = DateTime.UtcNow;

    public DateTime? NgayCan { get; set; }
    public DateTime? NgayGiao { get; set; }

    [Required, StringLength(255)]
    public string DiaChiGiao { get; set; } = null!;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PhiVanChuyen { get; set; }

    [Required]
    public int MaTrangThai { get; set; }
    public TrangThai TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    [StringLength(20)]
    public string? SoDienThoai { get; set; }

    // Navigation
    public ICollection<ChiTietHD> ChiTietHDs { get; set; } = new List<ChiTietHD>();
}

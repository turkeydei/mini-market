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

    public User? User { get; set; }

    [Required]
    [Display(Name = "Ngày Đặt")]
    public DateTime NgayDat { get; set; }

    public DateTime? NgayCan { get; set; }
    public DateTime? NgayGiao { get; set; }

    [Required]
    [StringLength(255)]
    public string? DiaChiGiao { get; set; }

    [Required]
    public float PhiVanChuyen { get; set; }

    [Required]
    public int MaTrangThai { get; set; }

    public TrangThai? TrangThai { get; set; }

    public string? GhiChu { get; set; }

    public string? SoDienThoai { get; set; }

    // Navigation Properties
    public ICollection<ChiTietHD>? ChiTietHDs { get; set; }
}

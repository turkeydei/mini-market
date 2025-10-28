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
    public DateTime NgayDat { get; set; } = DateTime.Now;

    public DateTime? NgayGiao { get; set; }

    [Required]
    [StringLength(255)]
    public string? DiaChiGiao { get; set; }

    [Required]
    public decimal TongTien { get; set; }

    [Required]
    public decimal PhiVanChuyen { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Trạng Thái")]
    public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Cancelled

    public string? GhiChu { get; set; }

    [StringLength(50)]
    public string? SoDienThoai { get; set; }

    // Navigation Properties
    public ICollection<ChiTietHD>? ChiTietHDs { get; set; }
    public PaymentTransaction? PaymentTransaction { get; set; }
}

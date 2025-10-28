using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("PaymentTransaction")]
public class PaymentTransaction
{
    [Key]
    public int MaGD { get; set; }

    [Required]
    public int MaHD { get; set; }

    public HoaDon? HoaDon { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Trạng Thái")]
    public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Cancelled

    [Required]
    [Display(Name = "Số Tiền")]
    public decimal SoTien { get; set; }

    [StringLength(50)]
    [Display(Name = "Phương Thức Thanh Toán")]
    public string? PhuongThucTT { get; set; } // COD, Bank Transfer, E-Wallet

    [Display(Name = "Ngày Tạo")]
    public DateTime NgayTao { get; set; } = DateTime.Now;

    [Display(Name = "Ngày Cập Nhật")]
    public DateTime? NgayCapNhat { get; set; }

    [StringLength(500)]
    public string? GhiChu { get; set; }
}


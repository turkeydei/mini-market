using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("User")]
public class User
{
    [Key]
    public int MaUser { get; set; }

    [Required]
    [StringLength(255)]
    [Display(Name = "Họ Tên")]
    public string? HoTen { get; set; }

    [Required]
    [StringLength(255)]
    public string? Email { get; set; }

    [Required]
    [StringLength(255)]
    public string? MatKhau { get; set; }

    [StringLength(50)]
    [Display(Name = "Điện Thoại")]
    public string? DienThoai { get; set; }

    // Phân biệt vai trò: Customer, Admin, Staff
    [Required]
    [StringLength(50)]
    public string VaiTro { get; set; } = "Customer";

    public int? GioiTinh { get; set; } // 0: Nam, 1: Nữ

    [Display(Name = "Ngày Sinh")]
    public DateTime? NgaySinh { get; set; }

    [StringLength(500)]
    [Display(Name = "Địa Chỉ")]
    public string? DiaChi { get; set; }

    // Navigation Properties
    public ICollection<HoaDon>? HoaDons { get; set; }
}

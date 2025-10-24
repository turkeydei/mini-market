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

    [Required, StringLength(255)]
    [Display(Name = "Họ Tên")]
    public string HoTen { get; set; } = null!;

    [Required, StringLength(255)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    // Lưu HASH mật khẩu tại đây (đừng lưu plain text)
    [Required, StringLength(255)]
    [Display(Name = "Mật Khẩu (Hash)")]
    public string MatKhau { get; set; } = null!;

    [StringLength(20)]
    [Display(Name = "Điện Thoại")]
    public string? DienThoai { get; set; }

    // 0: Khách hàng (mặc định), 1: Nhân viên (nếu sau này cần)
    public int VaiTro { get; set; } = 0;

    public int? GioiTinh { get; set; } // 0/1 nếu muốn
    public DateTime? NgaySinh { get; set; }

    [StringLength(255)]
    [Display(Name = "Địa Chỉ")]
    public string? DiaChi { get; set; }

    // Navigation
    public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}

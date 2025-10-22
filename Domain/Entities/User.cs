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

    // Phân biệt vai trò giữa Khách hàng và Nhân viên (0: Khách hàng, 1: Nhân viên)
    [Required]
    public int VaiTro { get; set; }

    // Các thuộc tính dành riêng cho Khách hàng
    public int? GioiTinh { get; set; } // Sử dụng kiểu int để lưu giá trị 0 hoặc 1

    [Display(Name = "Ngày Sinh")]
    public DateTime? NgaySinh { get; set; }

    [StringLength(255)]
    [Display(Name = "Địa Chỉ")]
    public string? DiaChi { get; set; }

    // Navigation Properties cho Khách hàng và Nhân viên
    public ICollection<HoaDon>? HoaDons { get; set; }
    public ICollection<PhanQuyen>? PhanQuyens { get; set; }
}

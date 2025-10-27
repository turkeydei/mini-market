using System.ComponentModel.DataAnnotations;

namespace Application.Features.DTOs;

public class CheckoutRequestDto
{
    [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
    [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
    public string DiaChiGiao { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string SoDienThoai { get; set; } = null!;

    public decimal PhiVanChuyen { get; set; } = 30000;

    public string? GhiChu { get; set; }

    public List<CheckoutItemDto> Items { get; set; } = new();
}


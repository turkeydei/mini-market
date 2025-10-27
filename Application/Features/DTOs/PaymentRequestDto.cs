using System.ComponentModel.DataAnnotations;

namespace Application.Features.DTOs;

public class PaymentRequestDto
{
    [Required]
    public int MaHD { get; set; }

    [Required]
    [StringLength(20)]
    public string Provider { get; set; } = "VNPAY";

    [Required]
    [Range(0.01, 999999999)]
    public decimal SoTien { get; set; }

    public string? MoTa { get; set; }
}


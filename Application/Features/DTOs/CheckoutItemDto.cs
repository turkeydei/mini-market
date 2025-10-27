namespace Application.Features.DTOs;

public class CheckoutItemDto
{
    public int MaHH { get; set; }
    public string TenHH { get; set; } = null!;
    public decimal DonGia { get; set; }
    public int SoLuong { get; set; }
    public decimal GiamGia { get; set; }
    public decimal ThanhTien => DonGia * SoLuong * (1 - GiamGia / 100);
    public string? Hinh { get; set; }
}


using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("ChiTietHD")]
public class ChiTietHD
{
    [Key]
    public int MaCT { get; set; }

    [Required]
    public int MaHD { get; set; }
    public HoaDon? HoaDon { get; set; }

    [Required]
    public int MaHH { get; set; }
    public HangHoa? HangHoa { get; set; }

    [Required]
    public decimal DonGia { get; set; }

    [Required]
    public int SoLuong { get; set; }

    public decimal GiamGia { get; set; } = 0;

    [NotMapped]
    public decimal ThanhTien => (DonGia * SoLuong) - GiamGia;
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

[Table("ChiTietHD")]
public class ChiTietHD
{
    [Key]
    public int MaCT { get; set; }

    [Required]
    public int MaHD { get; set; }
    public HoaDon HoaDon { get; set; } = null!;

    [Required]
    public int MaHH { get; set; }
    public HangHoa HangHoa { get; set; } = null!;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal DonGia { get; set; }

    [Required]
    public int SoLuong { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal GiamGia { get; set; } = 0m;
}
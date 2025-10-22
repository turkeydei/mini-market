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
    [Key] public int MaCT { get; set; }

    [Required] public int MaHD { get; set; }
    [Required] public HoaDon? HoaDon { get; set; }

    [Required] public int? MaHH { get; set; }
    [Required] public HangHoa? HangHoa { get; set; }

    [Required] public float DonGia { get; set; }

    [Required] public int SoLuong { get; set; }

    [Required] public float? GiamGia { get; set; }
}
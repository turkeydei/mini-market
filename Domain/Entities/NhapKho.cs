using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("NhapKho")]
    public class NhapKho
    {
        [Key]
        public int MaNK { get; set; }

        [Required]
        public int MaNCC { get; set; }
        public NhaCungCap? NhaCungCap { get; set; }

        [Required]
        public int MaHH { get; set; }
        public HangHoa? HangHoa { get; set; }

        [Required]
        [Display(Name = "Ngày Nhập")]
        public DateTime NgayNhap { get; set; }

        [Required]
        public int SoLuong { get; set; }

        [Required]
        public float GiaNhap { get; set; }
    }
}

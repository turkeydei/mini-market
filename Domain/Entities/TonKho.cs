using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("TonKho")]
    public class TonKho
    {
        [Key]
        public int MaHH { get; set; }
        public HangHoa? HangHoa { get; set; }

        public int? SoLuongTon { get; set; }

        [Required]
        [Display(Name = "Ngày Cập Nhật")]
        public DateTime? NgayCapNhat { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("ChiTietKhuyenMai")]
    public class ChiTietKhuyenMai
    {
       
        public int MaKM { get; set; }
        public KhuyenMai? KhuyenMai { get; set; }

    
        public int MaHH { get; set; }
        public HangHoa? HangHoa { get; set; }
    }
}

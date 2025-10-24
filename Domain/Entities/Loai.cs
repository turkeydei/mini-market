using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("Loai")]
    public class Loai
    {
        [Key]
        public int MaLoai { get; set; }

        [Required, StringLength(255)]
        [Display(Name = "Tên Loại")]
        public string TenLoai { get; set; } = null!;

        [Display(Name = "Mô Tả")]
        public string? MoTa { get; set; }

        // Navigation
        public ICollection<HangHoa> HangHoas { get; set; } = new List<HangHoa>();
    }
}

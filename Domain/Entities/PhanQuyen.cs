using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("PhanQuyen")]
    public class PhanQuyen
    {
        [Key]
        public int MaPQ { get; set; }

        [Required]
        public int MaNV { get; set; }
      

        [Required]
        public int MaTrang { get; set; }

        [Required]
        public bool Them { get; set; }

        [Required]
        public bool Sua { get; set; }

        [Required]
        public bool Xoa { get; set; }

        [Required]
        public bool Xem { get; set; }
    }
}

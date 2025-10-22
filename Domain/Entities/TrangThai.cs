using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("TrangThai")]
    public class TrangThai
    {
        [Key]
        public int MaTrangThai { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Tên Trạng Thái")]
        public string TenTrangThai { get; set; }

        [StringLength(255)]
        [Display(Name = "Mô Tả")]
        public string? MoTa { get; set; }

        // Navigation Propertiesa
        public ICollection<HoaDon>? HoaDons { get; set; }
    }
}

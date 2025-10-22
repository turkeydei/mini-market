using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace Domain.Entities
    {
        [Table("KhachHang")]
        public class KhachHang
        {
            [Key]
            public int MaKH { get; set; }

            [Required]
            [StringLength(255)]
            [Display(Name = "Họ Tên Khách Hàng")]
            public string? HoTen { get; set; }

            [StringLength(255)]
            public string? Email { get; set; }

            [Required]
            [StringLength(255)]
            public  string? MatKhau { get; set; }

            [Required]
            public bool GioiTinh { get; set; }

            [Required]
            [Display(Name = "Ngày Sinh")]
            public DateTime NgaySinh { get; set; }

            [StringLength(255)]
            [Display(Name = "Địa Chỉ")]
            public string? DiaChi { get; set; }

            [StringLength(50)]
            public  string? DienThoai { get; set; }

            [Required]
            public bool HieuLuc { get; set; }

            [Required]
            public int VaiTro { get; set; }

            public int? MaNV { get; set; }

            // Navigation Properties
            public ICollection<HoaDon>? HoaDons { get; set; }
        }
}

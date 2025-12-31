using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("ProductReview")]
public class ProductReview
{
    [Key]
    public int MaReview { get; set; }

    [Required]
    public int MaHH { get; set; }
    public HangHoa? HangHoa { get; set; }

    [Required]
    public int MaUser { get; set; }
    public User? User { get; set; }

    [Required]
    [Range(1, 5)]
    [Display(Name = "Đánh Giá")]
    public int Rating { get; set; } // 1-5 sao

    [StringLength(2000)]
    [Display(Name = "Nhận Xét")]
    public string? Comment { get; set; }

    [Display(Name = "Ngày Tạo")]
    public DateTime NgayTao { get; set; } = DateTime.Now;

    [Display(Name = "Đã Phê Duyệt")]
    public bool IsApproved { get; set; } = false; // Admin phê duyệt

    [StringLength(500)]
    [Display(Name = "Phản Hồi Của Admin")]
    public string? AdminResponse { get; set; } // Admin có thể phản hồi review
}


using Domain.Entities;

namespace WebShop.Models;

public class ProductDetailViewModel
{
    public HangHoa Product { get; set; } = null!;
    public IEnumerable<HangHoa> RelatedProducts { get; set; } = Enumerable.Empty<HangHoa>();
    public IEnumerable<ProductReview> Reviews { get; set; } = Enumerable.Empty<ProductReview>();
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public bool CanReview { get; set; } // User đã mua và chưa review
    public bool HasReviewed { get; set; } // User đã review rồi
}


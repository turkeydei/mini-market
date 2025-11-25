using Domain.Entities;

namespace WebShop.Models;

public class ProductDetailViewModel
{
    public HangHoa Product { get; set; } = null!;
    public IEnumerable<HangHoa> RelatedProducts { get; set; } = Enumerable.Empty<HangHoa>();
}


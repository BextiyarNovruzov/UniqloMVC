namespace FrontToBackMvc.Models;

public class Product:BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string CoverImage { get; set; } = null!;
    public int Quantity { get; set; }
    public int Discount { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellPrice { get; set; }
    public int CategoryId {  get; set; }
    public Category? Category { get; set; }
    public ICollection<ProductImage>? Images { get; set; }
}

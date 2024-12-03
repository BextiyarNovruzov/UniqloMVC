using System.ComponentModel.DataAnnotations;

namespace FrontToBackMvc.ViewModels.Products
{
    public class ProductCreateVM
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? CategoryName { get; set; } = null!;
        public IFormFile CoverPhoto { get; set; } 
        public IEnumerable<IFormFile> OtherFiles { get; set; } 
        public int Quantity { get; set; }
        public int Discount { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public int CategoryId { get; set; }
 
    }
}

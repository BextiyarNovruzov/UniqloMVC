using FrontToBackMvc.Models;

namespace FrontToBackMvc.ViewModels.Products
{
    public class ProductDetailsVM
    {
        public Product? Product { get; set; }

        public int UserRating { get; set; } = 5;
        public string UserComment { get; set; } = "";
    }
}

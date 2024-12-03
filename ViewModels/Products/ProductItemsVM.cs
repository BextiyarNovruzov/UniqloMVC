namespace FrontToBackMvc.ViewModels.Products
{
    public class ProductItemsVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string CoverImage { get; set; }
        public bool IsInStock { get; set; }
    }
}

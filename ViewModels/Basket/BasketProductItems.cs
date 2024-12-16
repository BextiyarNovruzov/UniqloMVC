namespace FrontToBackMvc.ViewModels.Basket
{
    public class BasketProductItems
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public int Count { get; set; }
        public string CoverImage { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Discount { get; set; }

    }
}

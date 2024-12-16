namespace FrontToBackMvc.ViewModels.Basket
{
    public class BasketProductsItemsVM
    {

        public int Id { get; set; }
        public int Count { get; set; }
        public BasketProductsItemsVM(int id)
        {
            Id = id;
            Count = 1;
        }


    }
}

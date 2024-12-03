using FrontToBackMvc.ViewModels.Products;
using FrontToBackMvc.ViewModels.Sliders;

namespace FrontToBackMvc.ViewModels.Common;

public class HomeVM
{
    public IEnumerable<SliderItemVM> Sliders {  get; set; } 
    public IEnumerable<ProductItemsVM> Products { get; set; }

}

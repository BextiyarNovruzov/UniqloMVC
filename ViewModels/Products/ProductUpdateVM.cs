using FrontToBackMvc.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace FrontToBackMvc.ViewModels.Products
{
    public class ProductUpdateVM
    {
        [Required(ErrorMessage = "Ad daxil edilməsi məcburidir.")]
        public string Name { get; set; }
        public string Description { get; set; } = null!;
       
        public IFormFile? CoverImage { get; set; }
        public string CoverFileUrl { get; set; }
        public IEnumerable<IFormFile>? OtherFiles { get; set; }
        public IEnumerable<ImageUrlAndIdVM>? OtherFileUrls{ get; set; }

        [Required(ErrorMessage = "Say daxil edilməsi məcburidir.")]
        public int Quantity { get; set; }

        public int Discount { get; set; }
        public decimal CostPrice { get; set; }
        [Required(ErrorMessage = "Qiymet daxil edilməsi məcburidir.")]
        public decimal SellPrice { get; set; }
    
        public int CategoryId { get; set; }
    
    }
}

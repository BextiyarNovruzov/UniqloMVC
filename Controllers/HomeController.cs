using FrontToBackMvc.DataAccess;
using FrontToBackMvc.ViewModels.Common;
using FrontToBackMvc.ViewModels.Products;
using FrontToBackMvc.ViewModels.Sliders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

namespace FrontToBackMvc.Controllers
{

    public class HomeController(UniqloDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            HomeVM vm = new HomeVM();

            vm.Sliders = await _context.Sliders
                .Where(x => !x.IsDeleted)
                .Select(x => new SliderItemVM
                {
                    Title = x.Title,
                    Subtitle = x.Subtitle,
                    Link = x.Link,
                    ImageUrl = x.ImageUrl
                }).ToListAsync();

            vm.Products = await _context.Products
                .Where(x=> !x.IsDeleted)
                .Select(x=> new ProductItemsVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.CostPrice,
                    CoverImage = x.CoverImage,
                    Discount = x.Discount,
                    IsInStock = x.Quantity > 0

                }).ToListAsync();

          return View(vm);  
        }
        public IActionResult About()
        {
            return View();
        }
        public async Task<IActionResult> AccessDenid()
        {

            return View();
        }
    }
}

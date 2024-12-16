using FrontToBackMvc.ViewModels.Basket;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FrontToBackMvc.Controllers
{
    public class BasketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            var BasketItems = JsonSerializer.Deserialize<List<BasketProductsItemsVM>>(Request.Cookies["basket"] ?? "[]");
            var item = BasketItems.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                item = new BasketProductsItemsVM(id);

                BasketItems.Add(item);
            }
            else
            {
                item.Count++;
            }

            Response.Cookies.Append("basket", JsonSerializer.Serialize(BasketItems));
            return Ok();
        }

        public async Task<IActionResult> RemoveProduct(int id)
        {
            var cookies = Request.Cookies["basket"];
            if (cookies == null) return BadRequest();
            var BasketItems = JsonSerializer.Deserialize<List<BasketProductsItemsVM>>(Request.Cookies["basket"] ?? "[]");
            var product = BasketItems.FirstOrDefault(x => x.Id == id);
            if(product != null) 
            {
                BasketItems.Remove(product);
            }
            var UpdatedBasket = JsonSerializer.Serialize(BasketItems);
            Response.Cookies.Append("basket",UpdatedBasket);
            return RedirectToAction("Index","Home");
        }
    }
}

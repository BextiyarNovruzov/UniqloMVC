using FrontToBackMvc.DataAccess;
using FrontToBackMvc.ViewModels.Basket;
using FrontToBackMvc.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FrontToBackMvc.Components
{
    public class HeaderViewComponent(UniqloDbContext _context):ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var BasketIds = JsonSerializer.Deserialize<List<BasketProductsItemsVM>>(Request.Cookies["basket"] ?? "[]");

            var Products = await _context.Products
                .Where(x => BasketIds.Select(y => y.Id).Any(y => y == x.Id))
                .Select(x => new BasketProductItems
                {
                    Id = x.Id,
                    Name = x.Name,
                    CoverImage = x.CoverImage,
                    Discount = x.Discount,
                    SellPrice = x.SellPrice,
                })
                .ToListAsync();
            foreach(var product in Products)
            {
                product.Count = BasketIds!.FirstOrDefault(x => x.Id == product.Id)!.Count;
            }
            return View(Products);
        }
    }
}

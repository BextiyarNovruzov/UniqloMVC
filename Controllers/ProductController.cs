using FrontToBackMvc.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace FrontToBackMvc.Controllers
{
    public class ProductController(UniqloDbContext _context) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) BadRequest();
            var data = await _context.Products
                .Where(x => x.Id == id && !x.IsDeleted)
                .Include(x => x.Images)
                .Include(x => x.Ragings)
                .Include(x => x.Comments)
                .FirstOrDefaultAsync();
            if (data == null) return NotFound();
            ViewBag.Rating = 5;
            ViewBag.Comment = " ";
            if (User.Identity?.IsAuthenticated ?? false)
            {
                string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
                int rating = await _context.ProductRatings.Where(x => x.UserId == userId && x.ProductId == id)
                    .Select(x => x.Rating).FirstOrDefaultAsync();
                ViewBag.Rating = rating == 0 ? 5 : rating;
                string content = await _context.Comments.Where(x => x.UserId == userId && x.ProductId == id)
                    .Select(x => x.Content).FirstOrDefaultAsync();
                ViewBag.Content = content;
            }
            return View(data);
        }

        public async Task<IActionResult> Rating(int productId, int rating)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
            var data = await _context.ProductRatings.Where(x => x.UserId == userId && x.ProductId == productId).FirstOrDefaultAsync();
            if (data is null)
            {
                await _context.ProductRatings.AddAsync(new Models.ProductRating
                {
                    UserId = userId,
                    ProductId = productId,
                    Rating = rating
                });
            }
            else
            {
                data.Rating = rating;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { Id = productId });
        }

        public async Task<IActionResult> Comment(int productId, string content)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
            var data = await _context.Comments.Where(x => x.UserId == userId && x.ProductId == productId)
                .FirstOrDefaultAsync();
            if (data is null)
            {
                await _context.Comments.AddAsync(new Models.Comment
                {
                    UserId = userId,
                    ProductId = productId,
                    Content = content

                });
            }
            else
            {
                content = data.Content;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { Id = productId });
        }
    }
}

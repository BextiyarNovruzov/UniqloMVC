using FrontToBackMvc.DataAccess;
using FrontToBackMvc.Models;
using FrontToBackMvc.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
            if (!id.HasValue) return BadRequest();
            var data = await _context.Products
                .Where(x => x.Id == id && !x.IsDeleted)
                .Include(x => x.Images)
                .Include(x => x.Ragings)
                .Include(x => x.Comments)
                .FirstOrDefaultAsync();
            if (data == null) return NotFound();
            ViewBag.Rating = 5;
            ViewBag.Comment = "";
            if (User.Identity?.IsAuthenticated ?? false)
            {
                string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
                int rating = await _context.ProductRatings.Where(x => x.UserId == userId && x.ProductId == id)
                    .Select(x => x.Rating).FirstOrDefaultAsync();
                ViewBag.Rating = rating == 0 ? 5 : rating;
                string comment = await _context.Comments.Where(x => x.UserId == userId && x.ProductId == id)
                    .Select(x => x.Content).FirstOrDefaultAsync();
                ViewBag.Comment = comment;
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

        public async Task<IActionResult> AddComment(int productId, ProductCommentVM vm)
        {

            //if (string.IsNullOrWhiteSpace(vm.Content))
            //{
            //    TempData["Error"] = "Comment bos ola/Bosluqla baslaya bilmez.";
            //    return RedirectToAction(nameof(Details), new { Id = productId });
            //}
            string UserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null)
            {
                TempData["Error"] = "User Tapilmadi";
                return RedirectToAction(nameof(Details), new { Id = productId });
            }
            //var ExtitingComment = await _context.Comments
            //    .FirstOrDefaultAsync(x => x.UserId == UserId && x.ProductId == productId);

            //if (ExtitingComment == null)
            //{
                var comment = new Comment
                {
                    UserId = vm.UserId,
                    Content = vm.Content,
                    ProductId = productId,
                    Author = User.Identity?.Name ?? "Anonim",
                   // UserEmail = User.Identity.
                };
                await _context.AddAsync(comment);
                await _context.SaveChangesAsync();
            
            //else
            //{
            //    TempData["Info"] = "Siz zaten comment etmisiniz";
            //}
            return RedirectToAction(nameof(Details), new { Id = productId });
        }
        

    }
}

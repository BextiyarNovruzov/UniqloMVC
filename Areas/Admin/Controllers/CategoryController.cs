using FrontToBackMvc.DataAccess;
using FrontToBackMvc.Models;
using FrontToBackMvc.ViewModels.Categories;
using FrontToBackMvc.ViewModels.Sliders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBackMvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CategoryController(UniqloDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateVM vm)
        {
            Category category = new Category
            { Name = vm.CategoryName };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Successfully Created";
            }
            return RedirectToAction(nameof(Create));
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return BadRequest();
            if (await _context.Categories.AnyAsync(x => x.Id == id.Value))
                _context.Categories.Remove(new Category { Id = id.Value});
                await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            Category? category = await _context.Categories.FindAsync(id.Value);

            CategoryUpdateVM vm = new CategoryUpdateVM();
            vm.Name = category.Name;

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int? id, CategoryUpdateVM vm)
        {
            if (!id.HasValue)
                return BadRequest("Invalid request. Category ID is required.");

            var data = await _context.Categories.FindAsync(id.Value);
            if (data is null)
                return NotFound("Category not found.");

            if (!ModelState.IsValid) return View(vm);

            data.Name = vm.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}

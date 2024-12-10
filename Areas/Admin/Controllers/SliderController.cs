using FrontToBackMvc.ViewModels.Sliders;
using FrontToBackMvc.DataAccess;
using Microsoft.AspNetCore.Mvc;
using FrontToBackMvc.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;
using FrontToBackMvc.Extentions;
using Microsoft.AspNetCore.Authorization;
using FrontToBackMvc.Helpers;

namespace FrontToBackMvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConstants.Slider)]
    public class SliderController(UniqloDbContext _context, IWebHostEnvironment web) : Controller
    {


        public async Task<IActionResult> Index()
        {

            return View(await _context.Sliders.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SliderCreateVM vm)
        {
            if (!ModelState.IsValid) return View();
            if (!vm.File.ContentType.StartsWith("image"))
            {
                ModelState.AddModelError("File", "Format type must be image");
            }
            if (vm.File.Length > 500 * 1024)
            {
                ModelState.AddModelError("File", "File size must be less than 500kb");
                return View();
            }

            string NewFileName = Path.GetRandomFileName() + Path.GetExtension(vm.File.FileName);

            using (Stream stream = System.IO.File.Create(Path.Combine(web.WebRootPath, "Images", "sliders", NewFileName)))
            {
                await vm.File.CopyToAsync(stream);
            }

            Slider slider = new Slider
            {
                ImageUrl = NewFileName,
                Link = vm.Link,
                Title = vm.Title,
                Subtitle = vm.Subtitle
            };
            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Slider? slider = await _context.Sliders.FindAsync(id);
            if (slider is null) return NotFound();
            SliderUpdateVM vm = new SliderUpdateVM();

            vm.Subtitle = slider.Subtitle;
            vm.Title = slider.Title;
            vm.Link = slider.Link;
            vm.ImageUrl = slider.ImageUrl;

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int? id, SliderUpdateVM vm)
        {
            if (!id.HasValue)
                return BadRequest("Invalid request. Slider ID is required.");

            var data = await _context.Sliders.FindAsync(id.Value);
            if (data == null)
                return NotFound("Slider not found.");

            if (!ModelState.IsValid)
            {
                return View(vm);
            }


            if (vm.File != null)
            {

                if (!vm.File.ContentType.StartsWith("image"))
                {
                    ModelState.AddModelError("File", "The file type must be an image.");
                }

                if (vm.File.Length > 500 * 1024)
                {
                    ModelState.AddModelError("File", "The file size must be less than 500kb.");
                }

              

                string oldFilePath = Path.Combine("wwwroot", "Images", "sliders", data.ImageUrl);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                
                    string newFileName = await vm.File.UploadAsync("wwwroot", "Images", "sliders");
                    data.ImageUrl = newFileName;
                
            }
         
            data.Title = vm.Title;
            data.Subtitle = vm.Subtitle;
            data.Link = vm.Link;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return BadRequest();
            if (await _context.Sliders.AnyAsync(x => x.Id == id.Value))
                _context.Sliders.Remove(new Slider { Id = id.Value });
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Hide(int? id)
        {
            if (id is null) return View();
            var data = await _context.Sliders.FindAsync(id);
            if (data is null) return View();
            data.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Show(int? id)
        {
            if (id is null) return View();
            var data = await _context.Sliders.FindAsync(id);
            if (data is null) return View();
            data.IsDeleted = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}

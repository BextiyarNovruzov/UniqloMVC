using FrontToBackMvc.DataAccess;
using FrontToBackMvc.Extentions;
using FrontToBackMvc.Migrations;
using FrontToBackMvc.Models;
using FrontToBackMvc.ViewModels.Products;
using FrontToBackMvc.ViewModels.Sliders;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using FrontToBackMvc.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using FrontToBackMvc.Helpers;

namespace FrontToBackMvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConstants.Product)]
    public class ProductController(UniqloDbContext _context, IWebHostEnvironment web) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.Include(x => x.Category).ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            if (vm.OtherFiles != null && vm.OtherFiles.Any())
            {
                if (!vm.OtherFiles.All(x => x.ContentType.StartsWith("image")))
                {
                    var InvalidFiles = vm.OtherFiles.Where(x => x.ContentType.StartsWith("image"))
                        .Select(x => x.FileName);
                    ModelState.AddModelError("OtherFiles", string.Join(",", InvalidFiles) + " are(is) not a image");

                }
                if (vm.OtherFiles.All(x => x.Length > 500 * 1024))
                {
                    var InvalidFiles = vm.OtherFiles.Where(x => x.Length < 500 * 1024)
                        .Select(x => x.FileName);
                    ModelState.AddModelError("OtherFiles", string.Join(", ", InvalidFiles) + "must be less than 500kb");
                }

            }

            if (vm.CoverPhoto != null)
            {

                if (!vm.CoverPhoto.ContentType.StartsWith("image"))
                {
                    ModelState.AddModelError("File", "Format type must be image");
                    return View(vm);
                }
                if (vm.CoverPhoto.Length > 500 * 1024)
                {
                    ModelState.AddModelError("File", "File size must be less than 500kb");
                    return View(vm);
                }
            }

            if (!ModelState.IsValid)
            {

                ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
                return View(vm);
            }

            string NewFileName = Path.GetRandomFileName() + Path.GetExtension(vm.CoverPhoto.FileName);

            using (Stream stream = System.IO.File.Create(Path.Combine(web.WebRootPath, "Images", "products", NewFileName)))
            {
                await vm.CoverPhoto.CopyToAsync(stream);
            }
            Product product = new Product
            {
                Name = vm.Name,
                CoverImage = NewFileName,
                Quantity = vm.Quantity,
                Discount = vm.Discount,
                SellPrice = vm.SellPrice,
                CategoryId = vm.CategoryId,
                CostPrice = vm.CostPrice,
                Description = vm.Description,
                Images = vm.OtherFiles.Select(x => new ProductImage { FileUrl = x.UploadAsync(web.WebRootPath, "Images", "products").Result }).ToList()
            };

            //List<ProductImage> productImages = new List<ProductImage>();
            //foreach (var image in vm.OtherFiles)
            //{
            //    string FileName = await image.UploadAsync(web.WebRootPath, "Images", "products");
            //    productImages.Add(new ProductImage
            //    {
            //        FileUrl = FileName,
            //        Product = product

            //    });

            //}

            //await _context.ProductImages.AddRangeAsync(productImages);

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id.HasValue) BadRequest();
            var product = await _context.Products.Where(x => x.Id == id.Value)
                .Select(x => new ProductUpdateVM
                {
                    Name = x.Name,
                    CostPrice = x.CostPrice,
                    SellPrice = x.SellPrice,
                    CategoryId = x.CategoryId,
                    Discount = x.Discount,
                    Quantity = x.Quantity,
                    Description = x.Description,
                    CoverFileUrl = x.CoverImage,
                    OtherFileUrls = x.Images.Select(y => new ViewModels.Common.ImageUrlAndIdVM
                    {
                        Url = y.FileUrl,
                        Id = y.Id,
                    })

                }).FirstOrDefaultAsync();
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            return View(product);

            //Product? product = await _context.Products.FindAsync(id);
            //if (product is null) return BadRequest();
            //ProductUpdateVM vm = new ProductUpdateVM();

            //vm.Name = product.Name;
            //vm.Quantity = product.Quantity;
            //vm.Discount = product.Discount;
            //vm.SellPrice = product.SellPrice;
            //vm.CategoryId = product.CategoryId;
            //vm.CostPrice = product.CostPrice;
            //vm.CoverImage = product.CoverImage;
            //vm.OtherFileUrls = product.Images.Select(x => x.FileUrl).FirstOrDefault();
            //ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();

            //return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int? id, ProductUpdateVM vm,ImageUrlAndIdVM Vm)
        {
            if (!id.HasValue) return BadRequest();
            var data = await _context.Products.FindAsync(id.Value);
          //  var images = await _context.ProductImages.FindAsync(id.Value);

            if (data is null) return NotFound();
//if (images is null) return NotFound();    


            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
                return View(vm);
            }





            //if (vm.OtherFiles != null && vm.OtherFiles.Any())
            //{
            //    if (!vm.OtherFiles.All(x => x.ContentType.StartsWith("image")))
            //    {
            //        var InvalidFiles = vm.OtherFiles.Where(x => x.ContentType.StartsWith("image"))
            //            .Select(x => x.FileName);
            //        ModelState.AddModelError("OtherFiles", string.Join(",", InvalidFiles) + " are(is) not a image");

            //    }
            //    if (vm.OtherFiles.All(x => x.Length > 500 * 1024))
            //    {
            //        var InvalidFiles = vm.OtherFiles.Where(x => x.Length < 500 * 1024)
            //            .Select(x => x.FileName);
            //        ModelState.AddModelError("OtherFiles", string.Join(", ", InvalidFiles) + "must be less than 500kb");
            //    }
            //    string oldFilePath = Path.Combine("wwwroot", "Images", "products", images.FileUrl);

            //    if (System.IO.File.Exists(oldFilePath))
            //    {
            //        System.IO.File.Delete(oldFilePath);
            //    }
            //    string NewFileUrl = await vm.OtherFiles.("wwwroot", "Images", "products");
            //    images.FileUrl = NewFileUrl;


            //}
            //data.
            if (vm.CoverImage != null)
            {

                if (!vm.CoverImage.ContentType.StartsWith("image"))
                {
                    ModelState.AddModelError("File", "Format type must be image");
                }
                if (vm.CoverImage.Length > 500 * 1024)
                {
                    ModelState.AddModelError("File", "File size must be less than 500kb");
                }
                string oldFilePath = Path.Combine("wwwroot", "Images", "products", data.CoverImage);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                string newFileName = await vm.CoverImage.UploadAsync("wwwroot", "Images", "products");
                data.CoverImage = newFileName;

            }
            data.Name = vm.Name;
            data.Discount = vm.Discount;
            data.Quantity = vm.Quantity;
            data.CostPrice = vm.CostPrice;
            data.SellPrice = vm.SellPrice;
            data.CategoryId = vm.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> DeleteImage(int? id)
        {
            if (!id.HasValue) BadRequest();
            var img = await _context.ProductImages.FindAsync(id.Value);
            if (img == null) return NotFound();
            _context.ProductImages.Remove(img);
            await _context.SaveChangesAsync();

            string path = Path.Combine(web.WebRootPath, "Images", "products", img.FileUrl);
            if (Path.Exists(path))
            {
                System.IO.File.Delete(path);
            }


            return Ok();
        }


        public async Task<IActionResult> Delete(int? id)
        {

            if (id is null) return BadRequest();
            if (await _context.Products.AnyAsync(x => x.Id == id))
                _context.Products.Remove(new Product { Id = id.Value });
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        //public async Task<IActionResult> Hide(int? id)
        //{
        //    if (id is null) return BadRequest();
        //    var product = await _context.Products.FindAsync(id);
        //    if (product is null) return BadRequest();
        //    product.IsDeleted = true;
        //    _context.SaveChanges();
        //    return RedirectToAction(nameof(Index));

        //}

        //public async Task<IActionResult> Show(int? id)
        //{
        //    if (id is null) return BadRequest();
        //    var product = _context.Products.Find(id);
        //    if (product is null) return View();
        //    product.IsDeleted = false;
        //    _context.SaveChanges();
        //    return RedirectToAction(nameof(Index));
        //}





    }
}

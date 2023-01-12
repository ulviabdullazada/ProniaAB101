using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProniaAB101.DAL;
using ProniaAB101.Models;
using ProniaAB101.Utilies.Extensions;
using ProniaAB101.ViewModels;
using System.Drawing.Printing;

namespace ProniaAB101.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        readonly AppDbContext _context;
        readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_context.Products.Include(p=>p.ProductColors).ThenInclude(pc=>pc.Color).Include(p=>p.ProductSizes).ThenInclude(ps=>ps.Size).Include(p=>p.ProductImages));
        }
        public IActionResult Create()
        {
            ViewBag.Colors = new SelectList(_context.Colors,"Id","Name");
            ViewBag.Sizes = new SelectList(_context.Sizes,nameof(Size.Id), nameof(Size.Name));
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateProductVM cp)
        {
            var coverImg = cp.CoverImage;
            var hoverImg = cp.HoverImage;
            var otherImgs = cp.OtherImages ?? new List<IFormFile>();
            string result = coverImg?.CheckValidate("image/", 300);
            if (result?.Length > 0)
            {
                ModelState.AddModelError("CoverImage", result);
            }
            result = hoverImg?.CheckValidate("image/", 300);
            if (result?.Length > 0)
            {
                ModelState.AddModelError("HoverImage", result);
            }
            foreach (IFormFile image in otherImgs)
            {
                result = image.CheckValidate("image/", 300);
                if (result?.Length > 0)
                {
                    ModelState.AddModelError("OtherImages", result);
                }
            }
            foreach (int colorId in (cp.ColorIds ?? new List<int>()))
            {
                if (!_context.Colors.Any(c=> c.Id == colorId))
                {
                    ModelState.AddModelError("ColorIds", "Get tullan");
                    break;
                }
            }
            foreach (int sizeId in cp.SizeIds)
            {
                if (!_context.Sizes.Any(s => s.Id == sizeId))
                {
                    ModelState.AddModelError("SizeIds", "Get tullan x2");
                    break;
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Colors = new SelectList(_context.Colors, "Id", "Name");
                ViewBag.Sizes = new SelectList(_context.Sizes, nameof(Size.Id), nameof(Size.Name));
                return View();
            }
            var sizes = _context.Sizes.Where(s => cp.SizeIds.Contains(s.Id));
            var colors = _context.Colors.Where(c => cp.ColorIds.Contains(c.Id));
            Product newProduct = new Product
            {
                Name = cp.Name,
                CostPrice = cp.CostPrice,
                SellPrice = cp.SellPrice,
                Description = cp.Description,
                Discount = cp.Discount,
                IsDeleted = false,
                SKU = "1"
            };
            List<ProductImage> images = new List<ProductImage>();
            images.Add(new ProductImage { ImageUrl = coverImg?.SaveFile(Path.Combine(_env.WebRootPath, "assets", "images", "product")), IsCover = true, Product = newProduct });
            if (hoverImg != null)
            {
                images.Add(new ProductImage { ImageUrl = hoverImg.SaveFile(Path.Combine(_env.WebRootPath, "assets", "images", "product")), IsCover = false, Product = newProduct });
            }
            foreach (var item in otherImgs)
            {
                images.Add(new ProductImage { ImageUrl = item?.SaveFile(Path.Combine(_env.WebRootPath, "assets", "images", "product")), IsCover = null, Product = newProduct });
            }
            newProduct.ProductImages = images;
            _context.Products.Add(newProduct);
            foreach (var item in colors)
            {
                _context.ProductColors.Add(new ProductColor { Product = newProduct, ColorId = item.Id});
            }
            foreach (var item in sizes)
            {
                _context.ProductSizes.Add(new ProductSize { Product = newProduct, SizeId = item.Id });
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UpdateProduct(int? id)
        {
            if (id == null) return BadRequest();
            Product product = _context.Products.Include(p => p.ProductColors).Include(p => p.ProductSizes).FirstOrDefault(p=>p.Id == id);
            if (product == null) return NotFound();
            UpdateProductVM updateProduct = new UpdateProductVM { 
                Id = product.Id, 
                Name = product.Name,
                Description = product.Description,
                Discount= product.Discount,
                SellPrice= product.SellPrice,
                CostPrice= product.CostPrice,
                ColorIds = new List<int>(),
                SizeIds = new List<int>(),
            };
            foreach (ProductColor color in product.ProductColors)
            {
                updateProduct.ColorIds.Add(color.ColorId);
            }
            foreach (var size in product.ProductSizes)
            {
                updateProduct.SizeIds.Add(size.SizeId);
            }
            ViewBag.Colors = new SelectList(_context.Colors,"Id","Name");
            ViewBag.Sizes = new SelectList(_context.Sizes,nameof(Size.Id), nameof(Size.Name));
            return View(updateProduct);
        }
        [HttpPost]
        public IActionResult UpdateProduct(int? id, UpdateProductVM updateProduct)
        {
            if (id == null) return NotFound();
            foreach (int colorId in (updateProduct.ColorIds ?? new List<int>()))
            {
                if (!_context.Colors.Any(c => c.Id == colorId))
                {
                    ModelState.AddModelError("ColorIds", "Get tullan");
                    break;
                }
            }
            foreach (int sizeId in (updateProduct.SizeIds ?? new List<int>()))
            {
                if (!_context.Sizes.Any(s => s.Id == sizeId))
                {
                    ModelState.AddModelError("SizeIds", "Get tullan x2");
                    break;
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Colors = new SelectList(_context.Colors, "Id", "Name");
                ViewBag.Sizes = new SelectList(_context.Sizes, nameof(Size.Id), nameof(Size.Name));
                return View();
            }
            var prod = _context.Products.Include(p => p.ProductColors).Include(p => p.ProductSizes).FirstOrDefault(p => p.Id == id);
            if (prod == null) return NotFound();
            foreach (var item in prod.ProductColors)
            {
                if (updateProduct.ColorIds.Contains(item.ColorId))
                {
                    updateProduct.ColorIds.Remove(item.ColorId);
                }
                else
                {
                    _context.ProductColors.Remove(item);
                }
            }
            foreach (var colorId in updateProduct.ColorIds)
            {
                _context.ProductColors.Add(new ProductColor { Product = prod, ColorId = colorId });
            }
            prod.Name = updateProduct.Name;
            prod.Discount = updateProduct.Discount;
            prod.CostPrice= updateProduct.CostPrice;
            prod.SellPrice = updateProduct.SellPrice;
            prod.Description = updateProduct.Description;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UpdateProductImage(int? id)
        {
            if (id == null) return BadRequest();
            var prod = _context.Products.Include(p => p.ProductImages).FirstOrDefault(p=>p.Id == id);
            if (prod == null) return NotFound();
            UpdateProductImageVM updateProductImage = new UpdateProductImageVM 
            {
                ProductImages = prod.ProductImages.Where(pi=>pi.IsCover == null)
            };
            return View(updateProductImage);
        }
        [HttpPost]
        public IActionResult UpdateProductImage(int? id, UpdateProductImageVM imageVM)
        {
            return View();
        }
        public IActionResult DeleteProductImage(int? id)
        {
            if (id == null) return BadRequest();
            var productImage = _context.ProductImages.Find(id);
            if (productImage == null) return NotFound();
            _context.ProductImages.Remove(productImage);
            _context.SaveChanges();
            return Ok();
        }
    }
}

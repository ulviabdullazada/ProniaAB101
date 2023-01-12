using Microsoft.AspNetCore.Mvc;
using ProniaAB101.DAL;
using ProniaAB101.Models;
using ProniaAB101.ViewModels;

namespace ProniaAB101.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class BrandController : Controller
    {
        readonly AppDbContext _context;
        readonly IWebHostEnvironment _env;
        public BrandController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Brands.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateBrandVM brandVm)
        {
            if (!ModelState.IsValid) return View();
            IFormFile file = brandVm.Image;
            if (!file.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Image", "qardas shekil yukle");
                return View();
            }
            if (!(brandVm.Image.Length/1024/1024 <2))
            {
                ModelState.AddModelError("Image", "qardas shekilin hecmi 2mbdan chox olmaz");
                return View();
            }
            string fileName= Guid.NewGuid() + file.FileName;
            using( var stream = new FileStream(Path.Combine(_env.WebRootPath,"assets","images","brand", fileName), FileMode.Create))
            {
                file.CopyTo(stream);
            }
            Brand brand = new Brand { ImageUrl= fileName };
            _context.Brands.Add(brand);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}

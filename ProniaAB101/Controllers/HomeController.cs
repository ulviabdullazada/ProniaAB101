using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaAB101.DAL;
using ProniaAB101.Models;
using ProniaAB101.ViewModels;

namespace ProniaAB101.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IQueryable<Product> products= _context.Products.Where(p => !p.IsDeleted).Include(p => p.ProductImages).AsQueryable();
            ViewBag.ProductCount = products.Count();
            HomeVM home = new HomeVM { Sliders = _context.Sliders.OrderBy(s=>s.Order), Brands = _context.Brands, FeaturedProducts = products.Take(4), LastestProducts = products.Take(4).OrderByDescending(p=>p.Id) };
            return View(home);
        }
        public IActionResult Shop()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LoadProducts(int skip = 4, int take = 4)
        {
            return PartialView("_ProductPartial", _context.Products.Where(p => !p.IsDeleted).Include(p => p.ProductImages).Skip(skip).Take(take));
        }
    }
}

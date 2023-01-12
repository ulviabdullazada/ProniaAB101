using Microsoft.AspNetCore.Mvc;

namespace ProniaAB101.Areas.Manage.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

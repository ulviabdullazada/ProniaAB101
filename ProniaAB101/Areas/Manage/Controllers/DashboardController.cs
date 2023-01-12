using Microsoft.AspNetCore.Mvc;
using ProniaAB101.DAL;

namespace ProniaAB101.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

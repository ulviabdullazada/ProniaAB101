using Microsoft.AspNetCore.Mvc;
using ProniaAB101.DAL;
using ProniaAB101.ViewModels;

namespace ProniaAB101.ViewComponents
{
    public class FooterViewComponent:ViewComponent
    {
        readonly AppDbContext _context;

        public FooterViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(_context.Settings.ToDictionary(s=>s.Key, s=>s.Value));
        }
    }
}

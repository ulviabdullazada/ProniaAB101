using Microsoft.AspNetCore.Mvc;
using ProniaAB101.DAL;

namespace ProniaAB101.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        readonly AppDbContext _context;

        public HeaderViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(View(_context.Settings.ToDictionary(s=>s.Key,s=>s.Value)));
        }
    }
}

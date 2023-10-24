using Microsoft.AspNetCore.Mvc;
using Quickfire_Bulletin.Services;
using System.Threading.Tasks;

namespace Quickfire_Bulletin.Controllers
{
    public class AdminController : Controller  // <- Class definition was missing
    {
        private readonly NewsService _newsService;

        public AdminController(NewsService newsService)
        {
            _newsService = newsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SeedDatabase()
        {
            try
            {
                await _newsService.SeedDatabaseAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                // Handle error
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
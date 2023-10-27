using Microsoft.AspNetCore.Mvc;
using Quickfire_Bulletin.Services;

namespace Quickfire_Bulletin.Controllers
{
    public class AdminController : Controller
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

        [HttpPost]
        public async Task<IActionResult> DeleteAllArticles()
        {
            try
            {
                await _newsService.EmptyArticleTableAsync();
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
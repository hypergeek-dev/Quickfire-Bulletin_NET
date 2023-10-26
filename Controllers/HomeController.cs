using Microsoft.AspNetCore.Mvc;
using Quickfire_Bulletin.Models;
using Quickfire_Bulletin.Services;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quickfire_Bulletin.Controllers
{
    public class HomeController : Controller
    {
        private readonly NewsService _newsService;

        public HomeController(NewsService newsService)
        {
            _newsService = newsService;
        }

        public async Task<IActionResult> Index()
        {
            List<NewsArticle> articles = await _newsService.GetArticlesAsync(includeComments: true);
            if (articles == null)
            {
                articles = new List<NewsArticle>();
            }
            return View(articles);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string articleId, string content)
        {
            string userName = User.Identity.Name;
            await _newsService.AddCommentAsync(articleId, content, userName);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> FetchCommentContent(int commentId)
        {
            var comment = await _newsService.GetCommentByIdAsync(commentId);
            if (comment == null)
            {
                return Json(new { success = false, message = "Comment not found" });
            }
            return Json(new { success = true, commentContent = comment.CommentContent });
        }
        [HttpGet]
        [Authorize(Policy = "MemberOrAdmin")]
        public async Task<IActionResult> EditComment(int commentId)
        {
            Comment comment = await _newsService.GetCommentByIdAsync(commentId); 
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment); 
        }

        [HttpPost]
        [Authorize(Policy = "MemberOrAdmin")]
        public async Task<IActionResult> EditComment(int commentId, string newContent)
        {
            await _newsService.EditCommentAsync(commentId, newContent);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Policy = "MemberOrAdmin")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            await _newsService.DeleteCommentAsync(commentId);
            return RedirectToAction("Index");
        }
    }
}

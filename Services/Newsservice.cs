using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Quickfire_Bulletin.Areas.Identity.Data;
using Quickfire_Bulletin.Models;
using Newtonsoft.Json;


namespace Quickfire_Bulletin.Services
{
    public class NewsService
    {
        private readonly ILogger<NewsService> _logger;
        private readonly Context _context;
        private readonly string _apiKey;

        public NewsService(ILogger<NewsService> logger, Context context, IOptions<MyAppSettings> settings)
        {
            _logger = logger;
            _context = context;
            _apiKey = settings.Value.APIKey;
        }

        public async Task AddCommentAsync(string articleId, string content, string userName)
        {
            var article = await _context.Articles.FindAsync(articleId);
            if (article == null)
            {
                return;
            }

            var comment = new Comment
            {
                CommentContent = content,
                Article = article,
                Name = userName 
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }


        public async Task EditCommentAsync(int commentId, string newContent)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                _logger.LogError($"Comment with ID {commentId} not found.");
                return;
            }

            comment.CommentContent = newContent;
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                _logger.LogError($"Comment with ID {commentId} not found.");
                return;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<NewsArticle>> GetArticlesAsync()
        {
            return await _context.Articles.ToListAsync();
        }

        public List<string> GroupIntoParagraphs(List<string> sentences, int n)
        {
            int numSentences = sentences.Count;
            List<string> paragraphs = new List<string>();

            int fullParagraphs = numSentences / n;
            int leftover = numSentences % n;

            for (int i = 0; i < fullParagraphs * n; i += n)
            {
                string paragraph = string.Join(" ", sentences.Skip(i).Take(n));
                paragraphs.Add(paragraph);
            }

            if (leftover > 0)
            {
                string paragraph = string.Join(" ", sentences.Skip(fullParagraphs * n));
                paragraphs.Add(paragraph);
            }

            return paragraphs;
        }

        public async Task SeedDatabaseAsync()
        {
            string apiUrl = $"https://newsdata.io/api/1/news?apikey={_apiKey}&country=gb&language=en";

            using var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                Models.NewsApiResponse apiResponse = JsonConvert.DeserializeObject<Models.NewsApiResponse>(content);

                if (apiResponse.Results is List<NewsArticle> newsArticles)
                {
                    foreach (var newsArticle in newsArticles)
                    {
                        List<string> sentences = SplitSentences(newsArticle.Content);
                        List<string> paragraphs = GroupIntoParagraphs(sentences, 5);
                        string formattedContent = string.Join("\n\n", paragraphs);

                        var article = new NewsArticle
                        {
                            ArticleId = newsArticle.ArticleId,
                            Title = newsArticle.Title,
                            Link = newsArticle.Link,
                            SourceId = newsArticle.SourceId,
                            SourcePriority = newsArticle.SourcePriority,
                            ImageUrl = newsArticle.ImageUrl,
                            VideoUrl = newsArticle.VideoUrl,
                            Description = newsArticle.Description,
                            PubDate = newsArticle.PubDate,
                            Content = formattedContent
                        };

                        _context.Articles.Add(article);
                    }

                    await _context.SaveChangesAsync();
                }
            }
        }

        private List<string> SplitSentences(string content)
        {
            return content.Split(new[] { ". ", "! ", "? " }, StringSplitOptions.None).ToList();
        }
    }
}

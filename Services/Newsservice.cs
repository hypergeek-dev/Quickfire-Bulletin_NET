using Microsoft.EntityFrameworkCore;
using Quickfire_Bulletin.Areas.Identity.Data;
using Quickfire_Bulletin.Models;
using Newtonsoft.Json;
using edu.stanford.nlp.pipeline;
using java.util;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.util;


namespace Quickfire_Bulletin.Services
{
    public class NewsService
    {
        private readonly ILogger<NewsService> _logger;
        private readonly Context _context;
        private readonly string _apiKey;
        private readonly StanfordCoreNLP pipeline;

        public NewsService(ILogger<NewsService> logger, Context context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _apiKey = configuration["ApiSettings:NewsApiKey"];

            var props = new Properties();
            props.setProperty("annotators", "tokenize, ssplit");
            pipeline = new StanfordCoreNLP(props);
        }

        public async Task AddCommentAsync(string articleId, string content, string userName)
        {
            var article = await _context.Articles.FindAsync(articleId);
            if (article == null) return;

            var comment = new Comment
            {
                CommentContent = content,
                Article = article,
                Name = userName,
                CreatedOn = DateTime.Now
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(int commentId)
        {
            return await _context.Comments.FindAsync(commentId);
        }

        public async Task EditCommentAsync(int commentId, string newContent)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null) return;

            comment.CommentContent = newContent;
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null) return;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<NewsArticle>> GetArticlesAsync(bool includeComments = false)
        {
            if (includeComments)
            {
                return await _context.Articles.Include(a => a.Comments).OrderBy(a => a.PubDate).ToListAsync();
            }

            return await _context.Articles.ToListAsync();
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
                        List<string> sentences = SplitSentencesWithCoreNLP(newsArticle.Content);
                        List<string> paragraphs = GroupIntoParagraphs(sentences, 5);
                        string processedContent = string.Join("\n\n", paragraphs);

                        var article = new NewsArticle
                        {
                            ArticleId = newsArticle.ArticleId,
                            Title = newsArticle.Title,
                            Link = newsArticle.Link,
                            SourceId = newsArticle.SourceId,
                            Description = newsArticle.Description,
                            PubDate = newsArticle.PubDate,
                            Content = processedContent
                        };

                        _context.Articles.Add(article);
                    }

                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task EmptyArticleTableAsync()
        {
            _context.Articles.RemoveRange(_context.Articles);
            await _context.SaveChangesAsync();
        }

        public List<string> SplitSentencesWithCoreNLP(string content)
        {
            var annotation = new edu.stanford.nlp.pipeline.Annotation(content);
            pipeline.annotate(annotation);

            var sentences = new List<string>();
            var coreMap = annotation.get(typeof(CoreAnnotations.SentencesAnnotation)) as ArrayList;
            if (coreMap != null)
            {
                foreach (CoreMap sentence in coreMap)
                {
                    sentences.Add(sentence.ToString());
                }
            }
            else
            {
                _logger.LogError("CoreMap is null. Sentence splitting may have failed.");
            }

            return sentences;
        }

        public List<string> GroupIntoParagraphs(List<string> sentences, int sentencesPerParagraph)
        {
            Console.WriteLine("GroupIntoParagraphs called");
            Console.WriteLine($"Sentences count: {sentences.Count}, Sentences per paragraph: {sentencesPerParagraph}");

           
            List<string> filteredSentences = new List<string>(sentences);
            string[] unwantedKeywords = { "e-mail", "shares", "Add comment", "View comments", "Facebook", "Comments", "Newest", "Oldest", "Best rated", "Worst rated", "moderated", "Enter your comment", "agree to our", "Submit", "Clear", "Close", "MailOnline", "No", "Yes", "View all", "top stories" };

         
            if (filteredSentences.Count >= 2)
            {
                for (int i = filteredSentences.Count - 1; i >= filteredSentences.Count - 2; i--)
                {
                    if (unwantedKeywords.Any(keyword => filteredSentences[i].Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                    {
                        filteredSentences.RemoveAt(i);
                    }
                }
            }

        
            int numSentences = filteredSentences.Count;
            List<string> paragraphs = new List<string>();

        
            int fullParagraphs = numSentences / sentencesPerParagraph;
            int leftover = numSentences % sentencesPerParagraph;

 
            for (int i = 0; i < fullParagraphs * sentencesPerParagraph; i += sentencesPerParagraph)
            {
                string paragraph = string.Join(" ", filteredSentences.Skip(i).Take(sentencesPerParagraph));
                paragraphs.Add(paragraph);
            }

            if (leftover > 0)
            {
                string lastParagraph = string.Join(" ", filteredSentences.Skip(fullParagraphs * sentencesPerParagraph));
                paragraphs.Add(lastParagraph);
            }


            if (paragraphs.Count > 0)
            {
                string lastParagraph = paragraphs[^1];
                if (!lastParagraph.EndsWith(".") && !lastParagraph.EndsWith("\""))
                {
                    paragraphs[^1] += ".";
                }
                else if (lastParagraph.EndsWith("\""))
                {
                    int lastIndex = lastParagraph.LastIndexOf("\"");
                    if (lastIndex > 0 && lastParagraph[lastIndex - 1] != '.')
                    {
                        paragraphs[^1] = lastParagraph.Insert(lastIndex, ".");
                    }
                }
            }



            Console.WriteLine($"Paragraphs count: {paragraphs.Count}");

            return paragraphs;
        }

    }
}

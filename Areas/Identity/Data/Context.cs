using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Quickfire_Bulletin.Models;
using Newtonsoft.Json;

namespace Quickfire_Bulletin.Areas.Identity.Data
{
    public class Context : IdentityDbContext<Quickfire_BulletinUser>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<NewsArticle> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Article)
                .WithMany(na => na.Comments)
                .HasForeignKey(c => c.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class NewsApiResponse
    {
        [JsonProperty("status")] public string Status { get; set; }

        [JsonProperty("totalResults")] public int TotalResults { get; set; }

        [JsonProperty("results")] public List<NewsArticle> Results { get; set; }
    }

}

using Newtonsoft.Json;

namespace Quickfire_Bulletin.Models
{
    public class NewsApiResponse
    {
        [JsonProperty("status")] public string Status { get; set; }

        [JsonProperty("totalResults")] public int TotalResults { get; set; }

        [JsonProperty("results")] public List<NewsArticle> Results { get; set; }
    }



}
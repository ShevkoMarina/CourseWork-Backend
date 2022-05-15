using Newtonsoft.Json;

namespace CourseBack.Models.serpapi
{
    public class ImageResult
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        //link to website
        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("is_product")]
        public bool IsProduct { get; set; }

        //image url
        [JsonProperty("original")]
        public string Original { get; set; }
    }
}

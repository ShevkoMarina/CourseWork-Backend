using Newtonsoft.Json;
using System.Collections.Generic;

namespace CourseBack.Models.serpapi
{
    public class SimilarItemsResponse
    {
        [JsonProperty("images_results")]
        public IEnumerable<ImageResult> ImagesResults;
    }
}

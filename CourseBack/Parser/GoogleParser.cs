using CourseBack.Models;
using CourseBack.Models.serpapi;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CourseBack.Parser
{
    public class GoogleParser
    {
        private readonly string imageUrl;
        private const string baseUrl = "https://www.google.com/searchbyimage?hl=ru&image_url=";
        private const string serpapiUrl = "https://serpapi.com/search.json?engine=google";
        private HtmlDocument document = new HtmlDocument();
        private Guid userId;
        private string category;
        private readonly string apiKey;

        public static HttpClient Client { get; set; } = new HttpClient();

        public GoogleParser(string imageUrl, Guid userId, string category, string apiKey)
        {
            this.imageUrl = imageUrl;
            this.userId = userId;
            this.category = category;
            this.apiKey = apiKey;
        }

        private async Task<string> GetSource(string url)
        {
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.163 Safari/537.36 OPR/67.0.3575.137");

            try
            {
                HttpResponseMessage response = await Client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                throw new Exception("Parser request error");
            }
        }

        public async Task<IReadOnlyCollection<SavedItem>> GetData()
        {
            document.LoadHtml(await GetSource($"{baseUrl}{imageUrl}"));
            List<SavedItem> items = new List<SavedItem>();

            var similarImagesLink = document?.DocumentNode?.SelectSingleNode("//a[@class='czzyk']")?.GetAttributeValue("href", null);

            if (similarImagesLink == null)
                return items;

            similarImagesLink = similarImagesLink.Replace("&amp;", "&").Replace("/search?", "");
            similarImagesLink = $"{serpapiUrl}&hl=en&api_key={apiKey}&{similarImagesLink}";

            var similarProducts = await FindSimilarProducts(similarImagesLink);

            foreach (var product in similarProducts.ImagesResults)
            {
                items.Add(new SavedItem()
                {
                    Id = Guid.NewGuid(),
                    Name = product.Title,
                    ImageUrl = product.Original,
                    WebUrl = product.Link,
                    Price = string.Empty,
                    Category = category,
                    UserId = userId
                });
            }

            return items;
        }

        private async Task<SimilarItemsResponse> FindSimilarProducts(string requestUrl)
        {
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await Client.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var jsonResult = JObject.Parse(json);

            if (jsonResult["error"] != null)
                return new SimilarItemsResponse();

            var similarItemsResponse = jsonResult.ToObject<SimilarItemsResponse>();
            similarItemsResponse.ImagesResults = similarItemsResponse.ImagesResults.ToList().Where(x => x.IsProduct);

            return similarItemsResponse;
        }
    }
}

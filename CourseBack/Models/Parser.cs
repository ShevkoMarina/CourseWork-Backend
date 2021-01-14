using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Net.Http;

namespace CourseBack.Models
{
    // нормально ли что здесь есть логика? 
    public class Parser
    {
        private string imageUrl;
        private const string baseUrl = "https://yandex.ru/images/search?rpt=imageview&url=";
        private HtmlDocument document;
        private Guid userId;

        public static HttpClient Client { get; set; } = new HttpClient();

        public Parser(string imageUrl, Guid userId)
        {
            this.imageUrl = imageUrl;
            this.userId = userId;
        }

        private async Task<string> GetSource(string imageUrl)
        {
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla / 5.0 (Windows NT 6.3; WOW64; rv: 31.0) Gecko / 20100101 Firefox / 31.0");

            try
            {
                HttpResponseMessage response = await Client.GetAsync(baseUrl + imageUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<HtmlDocument> GetDocument()
        {
            var source = await GetSource(imageUrl);
            document = new HtmlDocument();
            document.LoadHtml(source);
            return document;
        }

        //   переназвать моель
        public async Task<List<SavedItemRequest>> GetData()
        {
            document = await GetDocument();
            List<SavedItemRequest> items = new List<SavedItemRequest>();

            var article = document?.DocumentNode?.SelectSingleNode(".//div[@class='CbirItem CbirMarketProducts CbirMarketProducts_carousel']");
            var itemsPanel = article?.SelectSingleNode(".//div[@class='CbirMarketProducts-Items']");

            foreach (HtmlNode itemNode in itemsPanel.SelectNodes(".//div[@class='CbirMarketProducts-Item']"))
            {
                var imageBlock = itemNode.SelectSingleNode(".//div[@class='Thumb Thumb_type_block MarketProduct-Thumb']");
                string imagePath = imageBlock.SelectSingleNode(".//div").Attributes["style"]?.Value;
                imagePath = imagePath.Replace("height:110px;background-image:url(", "");
                imagePath = "https:" + imagePath.Replace(")", "");

                items.Add(new SavedItemRequest()
                {
                    UserId = userId,
                    ImageUrl = imagePath,
                    Price = itemNode.SelectSingleNode(".//span[@class='PriceValue']").InnerText, // знак вопроса?
                    // прилепить доллар? 
                    Name = itemNode.SelectSingleNode(".//div[@class='MarketProduct-Title']").InnerText,
                    WebUrl = itemNode.SelectSingleNode(".//a[@class='Link MarketProduct-Link']").Attributes["href"]?.Value
                });
            }

            return items;
        }
    }
}

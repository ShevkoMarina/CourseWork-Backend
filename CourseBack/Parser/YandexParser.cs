using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Net.Http;
using CourseBack.Models;

namespace CourseBack.Parser
{
    public class YandexParser
    {
        private string imageUrl;
        private const string baseUrl = "https://yandex.ru/images/search?rpt=imageview&url="; 
        private HtmlDocument document;
        private Guid userId;
        private string category;

        public static HttpClient Client { get; set; } = new HttpClient();

        public YandexParser(string imageUrl, Guid userId, string category)
        {
            this.imageUrl = imageUrl;
            this.userId = userId;
            this.category = category;
        }

        private async Task<string> GetSource(string imageUrl)
        {
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.163 Safari/537.36 OPR/67.0.3575.137");

            try
            {
                String newUrl = baseUrl + imageUrl;
                HttpResponseMessage response = await Client.GetAsync(newUrl);
                Console.WriteLine(response.Content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                throw new Exception("Parser request error");
            }
        }

        private async Task<HtmlDocument> GetDocument()
        {
            var source = await GetSource(imageUrl);
            document = new HtmlDocument();
            document.LoadHtml(source);
            return document;
        }


        public async Task<IReadOnlyCollection<SavedItem>> GetData()
        {
            document = await GetDocument();
            List<SavedItem> items = new List<SavedItem>();

            try
            {
                var article = document?.DocumentNode?.SelectSingleNode(".//div[@class='CbirItem CbirMarketProducts CbirMarketProducts_grid']");
                if (article == null)
                {
                    return null;
                }
                var itemsPanel = article?.SelectSingleNode(".//div[@class='CbirMarketProducts-Items']");

                foreach (HtmlNode itemNode in itemsPanel.SelectNodes(".//div[@class='CbirMarketProducts-Item CbirMarketProducts-Item_type_product']"))
                {
                    try
                    {
                      
                        var imageBlock = itemNode.SelectSingleNode(".//div[@class='Thumb Thumb_type_block CbirProduct-Thumb']");
                        string imagePath = imageBlock?.SelectSingleNode(".//div")?.Attributes["style"]?.Value;
                        imagePath = imagePath?.Replace("height:110px;background-image:url(", "");
                        imagePath = "https:" + imagePath?.Replace(")", "");

                        var item = new SavedItem()
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            ImageUrl = imagePath ?? "",
                            Price = itemNode.SelectSingleNode(".//span[@class='PriceValue']")?.InnerText ?? "",                                                                                    
                            Name = itemNode.SelectSingleNode(".//div[@class='CbirProduct-Title']")?.InnerText ?? "",
                            WebUrl = itemNode?.SelectSingleNode(".//a[@class='Link Link_theme_outer CbirProduct-ShopDomainLink']").Attributes["href"].Value ?? "",
                            Category = category
                        };

                        items.Add(item);
                    }
                    catch(Exception)
                    {
                        continue;
                    }
                }

                return items;
            }
            catch (Exception)
            {
                throw new NullReferenceException(document.Text);
            }
        }
    }
}

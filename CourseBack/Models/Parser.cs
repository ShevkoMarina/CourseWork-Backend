﻿using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Mail;

namespace CourseBack.Models
{
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


        public async Task<IReadOnlyCollection<RecognizeItemRequest>> GetData()
        {
            document = await GetDocument();
            List<RecognizeItemRequest> items = new List<RecognizeItemRequest>();

            try
            {
                var article = document?.DocumentNode?.SelectSingleNode(".//div[@class='CbirItem CbirMarketProducts CbirMarketProducts_carousel']");
                var itemsPanel = article?.SelectSingleNode(".//div[@class='CbirMarketProducts-Items']");

                foreach (HtmlNode itemNode in itemsPanel.SelectNodes(".//div[@class='CbirMarketProducts-Item']"))
                {
                    try
                    {
                        var imageBlock = itemNode.SelectSingleNode(".//div[@class='Thumb Thumb_type_block MarketProduct-Thumb']");
                        string imagePath = imageBlock.SelectSingleNode(".//div").Attributes["style"]?.Value;
                        imagePath = imagePath.Replace("height:110px;background-image:url(", "");
                        imagePath = "https:" + imagePath.Replace(")", "");
                        // эксешн если предмета нет в  продаже - и цены у него нет

                        var item = new RecognizeItemRequest()
                        {
                            UserId = userId,
                            ImageUrl = imagePath,
                            Price = itemNode.SelectSingleNode(".//span[@class='PriceValue']")?.InnerText, // знаки вопроса                                                                                   
                            Name = itemNode.SelectSingleNode(".//div[@class='MarketProduct-Title']")?.InnerText,
                            WebUrl = itemNode.SelectSingleNode(".//a[@class='Link MarketProduct-Link']")?.Attributes["href"]?.Value
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

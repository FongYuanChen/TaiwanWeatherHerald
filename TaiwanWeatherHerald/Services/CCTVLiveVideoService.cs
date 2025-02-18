using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TaiwanWeatherHerald.Interfaces;
using TaiwanWeatherHerald.Models;

namespace TaiwanWeatherHerald.Services
{
    public class CCTVLiveVideoService : ICCTVLiveVideoService
    {
        private readonly string _baseUrl = "https://www.twipcam.com";

        /// <summary>
        /// 根據指定的經緯度獲取附近的CCTV監視器即時影像資訊
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public async Task<List<CCTVLiveVideoInfo>> GetCCTVLiveVideoInfos(double longitude, double latitude)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var url = $"{_baseUrl}/api/v1/query-cam-list-by-coordinate?lat={latitude}&lon={longitude}";
                    var htmlContent = await httpClient.GetStringAsync(url);
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(htmlContent);

                    // 選擇所有包含"/cam/"的 <a> 節點，獲取CCTV監視器資訊連結
                    var cctvLiveVideoInfoLinks = htmlDoc.DocumentNode
                                                        .SelectNodes("//a[@href]")
                                                        .Select(node => node.GetAttributeValue("href", null))
                                                        .Where(href => !string.IsNullOrEmpty(href) && href.Contains("/cam/"))
                                                        .Select(href => _baseUrl + href)
                                                        .ToList();

                    var cctvLiveVideoInfoTasks = cctvLiveVideoInfoLinks.Select(cctvLiveVideoInfoLink => Task.Run(async () =>
                    {
                        try
                        {
                            var cctvLiveVideoInfoHtml = await httpClient.GetStringAsync(cctvLiveVideoInfoLink);
                            var cctvLiveVideoInfo = ExtractCCTVLiveVideoInfo(cctvLiveVideoInfoHtml);
                            return cctvLiveVideoInfo;
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    }));

                    return (await Task.WhenAll(cctvLiveVideoInfoTasks)).Where(info => info != null).ToList();
                }
                catch (Exception)
                {
                    return new List<CCTVLiveVideoInfo>();
                }
            }
        }

        /// <summary>
        /// 從HTML內容中提取CCTV監視器的即時影像資訊
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        private static CCTVLiveVideoInfo ExtractCCTVLiveVideoInfo(string htmlContent)
        {
            var htmlDoc = new HtmlDocument() { OptionWriteEmptyNodes = true };
            htmlDoc.LoadHtml(htmlContent);

            // 提取監視器名稱
            var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//title");
            var cctvName = titleNode.InnerText.Replace("即時影像 - 台灣即時影像監視器", "").Trim();

            // 提取經緯度
            var longitudeNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(text(), '經度')]");
            var longitudeText = longitudeNode.InnerText.Replace("經度:", "").Trim();
            var latitudeNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(text(), '緯度')]");
            var latitudeText = latitudeNode.InnerText.Replace("緯度:", "").Trim();

            // 提取即時影像網址
            var videoUrl = string.Empty;
            if (string.IsNullOrEmpty(videoUrl))
            {
                // 提取 <img> 節點
                var imgNode = htmlDoc.DocumentNode.SelectSingleNode("//img[@class='video_obj']");
                if (imgNode != null)
                {
                    videoUrl = imgNode.GetAttributeValue("data-src", null) ?? imgNode.GetAttributeValue("src", null);
                }
            }
            if (string.IsNullOrEmpty(videoUrl))
            {
                // 提取 <iframe> 節點
                var iframeNode = htmlDoc.DocumentNode.SelectSingleNode("//iframe[@src]");
                if (iframeNode != null)
                {
                    videoUrl = iframeNode.GetAttributeValue("src", null);
                }
            }
            if (string.IsNullOrEmpty(videoUrl))
            {
                // 提取 <video> 節點
                var videoNode = htmlDoc.DocumentNode.SelectSingleNode("//video[@src]");
                if (videoNode != null)
                {
                    videoUrl = videoNode.GetAttributeValue("src", null);
                }
            }

            return new CCTVLiveVideoInfo
            {
                Name = cctvName,
                Longitude = Convert.ToDouble(longitudeText),
                Latitude = Convert.ToDouble(latitudeText),
                Url = videoUrl
            };
        }
    }
}

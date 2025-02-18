using System.Collections.Generic;

namespace TaiwanWeatherHerald.Models
{
    /// <summary>
    /// 旅遊地點天氣訊息
    /// </summary>
    public class TripLocationWeatherInfo
    {
        /// <summary>
        /// 地點名稱
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 緯度座標
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 經度座標
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 一週天氣預報訊息
        /// </summary>
        public List<WeatherForecastInfo> WeeklyForecastInfo { get; set; }

        /// <summary>
        /// 72小時天氣預報訊息
        /// </summary>
        public List<WeatherForecastInfo> Hourly72ForecastInfo { get; set; }

        /// <summary>
        /// 天氣小幫手訊息
        /// </summary>
        public WeatherAssistantInfo AssistantInfo { get; set; }

        /// <summary>
        /// 天氣圖資訊息
        /// </summary>
        public WeatherMapInfo WeatherMapInfo { get; set; }
    }
}

using System.Collections.Generic;
using TaiwanWeatherHerald.Models;

namespace TaiwanWeatherHerald.Services.WeatherService.Models
{
    /// <summary>
    /// 內部天氣預報訊息
    /// </summary>
    public class InternalForecastInfo
    {
        /// <summary>
        /// 城市名稱
        /// </summary>
        public string City { get; set; }

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
        /// 天氣預報訊息
        /// </summary>
        public List<WeatherForecastInfo> WeatherForecastInfos { get; set; }
    }
}

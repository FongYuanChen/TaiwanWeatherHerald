namespace TaiwanWeatherHerald.Models
{
    /// <summary>
    /// 天氣圖資訊息
    /// </summary>
    public class WeatherMapInfo
    {
        /// <summary>
        /// 衛星雲圖
        /// </summary>
        public string SatelliteCloudMapUrl { get; set; }

        /// <summary>
        /// 雷達整合回波圖
        /// </summary>
        public string RainRadarMapUrl { get; set; }

        /// <summary>
        /// 累積雨量圖
        /// </summary>
        public string AccumulatedRainfallMapUrl { get; set; }

        /// <summary>
        /// 溫度分布圖
        /// </summary>
        public string TemperatureDistributionMapUrl { get; set; }
    }
}

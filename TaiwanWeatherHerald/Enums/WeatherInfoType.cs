namespace TaiwanWeatherHerald.Enums
{
    /// <summary>
    /// 天氣資訊類型
    /// </summary>
    public enum WeatherInfoType
    {
        /// <summary>
        /// 72小時天氣預報
        /// </summary>
        Hourly72Forecast = 1,

        /// <summary>
        /// 一週天氣預報
        /// </summary>
        WeeklyForecast = 2,

        /// <summary>
        /// 天氣小幫手
        /// </summary>
        WeatherAssistant = 3,

        /// <summary>
        /// 天氣圖資
        /// </summary>
        WeatherMap = 4,

        /// <summary>
        /// 監視器即時影像
        /// </summary>
        CCTVLiveVideo = 5,
    }
}

namespace TaiwanWeatherHerald.Models
{
    /// <summary>
    /// 監視機即時影像訊息
    /// </summary>
    public class CCTVLiveVideoInfo
    {
        /// <summary>
        /// 名稱或識別標籤
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 緯度座標
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 經度座標
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 即時影像網址
        /// </summary>
        public string Url { get; set; }
    }
}

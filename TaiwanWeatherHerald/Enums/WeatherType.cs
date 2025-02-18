namespace TaiwanWeatherHerald.Enums
{
    /// <summary>
    /// 天氣類型
    /// </summary>
    public enum WeatherType
    {
        /// <summary>
        /// 晴朗，天空無雲或僅有少量雲層
        /// </summary>
        Clear = 1,

        /// <summary>
        /// 多雲，天空被部分或完全雲層覆蓋
        /// </summary>
        Cloudy = 2,

        /// <summary>
        /// 下雨，包括小雨或大雨
        /// </summary>
        Rainy = 3,

        /// <summary>
        /// 暴風雨，可能伴隨雷電、大風和強降雨
        /// </summary>
        Stormy = 4,

        /// <summary>
        /// 有霧，能見度較低
        /// </summary>
        Foggy = 5,

        /// <summary>
        /// 下雪，包括小雪或暴雪
        /// </summary>
        Snowy = 6
    }
}

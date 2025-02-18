using System;
using TaiwanWeatherHerald.Enums;

namespace TaiwanWeatherHerald.Models
{
    /// <summary>
    /// 天氣預報訊息
    /// </summary>
    public class WeatherForecastInfo
    {
        /// <summary>
        /// 白天或夜晚，用於標示當前時間所屬的時段
        /// </summary>
        public DayNightType DayNightType { get; set; }

        /// <summary>
        /// 日期與時間
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// 天氣現象
        /// </summary>
        public WeatherType Weather { get; set; }

        /// <summary>
        /// 天氣現象描述
        /// </summary>
        public string WeatherDescription { get; set; }

        /// <summary>
        /// 天氣現象綜合描述
        /// </summary>
        public string WeatherDetailDescription { get; set; }

        /// <summary>
        /// 溫度，單位:°C
        /// </summary>
        public int Temperature { get; set; }

        /// <summary>
        /// 蒲福風級，單位:蒲福風級
        /// </summary>
        public int? WindBeaufortScale { get; set; }

        /// <summary>
        /// 風向
        /// </summary>
        public WindDirectionType? WindDirection { get; set; }

        /// <summary>
        /// 降雨機率，單位:%
        /// </summary>
        public int? ProbabilityOfPrecipitation { get; set; }

        /// <summary>
        /// 平均相對濕度，單位:%
        /// </summary>
        public int? RelativeHumidity { get; set; }

        /// <summary>
        /// 紫外線指數
        /// </summary>
        public int? UVIndex { get; set; }
    }
}

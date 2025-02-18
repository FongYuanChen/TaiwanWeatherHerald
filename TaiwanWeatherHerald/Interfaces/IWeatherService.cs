using System.Collections.Generic;
using System.Threading.Tasks;
using TaiwanWeatherHerald.Enums;
using TaiwanWeatherHerald.Models;

namespace TaiwanWeatherHerald.Interfaces
{
    /// <summary>
    /// 提供天氣相關功能的服務介面
    /// </summary>
    public interface IWeatherService
    {
        /// <summary>
        /// 獲取旅遊地點天氣訊息
        /// </summary>
        /// <param name="tripType"></param>
        /// <returns></returns>
        public Task<List<TripLocationWeatherInfo>> GetTripLocationWeatherInfosAsync(TripType tripType);
    }
}

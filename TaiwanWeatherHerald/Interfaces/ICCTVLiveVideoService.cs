using System.Collections.Generic;
using System.Threading.Tasks;
using TaiwanWeatherHerald.Models;

namespace TaiwanWeatherHerald.Interfaces
{
    /// <summary>
    /// 提供CCTV監視器相關功能的服務介面
    /// </summary>
    public interface ICCTVLiveVideoService
    {
        /// <summary>
        /// 根據指定的經緯度獲取附近的CCTV監視器即時影像資訊
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public Task<List<CCTVLiveVideoInfo>> GetCCTVLiveVideoInfos(double longitude, double latitude);
    }
}

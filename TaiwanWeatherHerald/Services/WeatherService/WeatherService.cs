using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using TaiwanWeatherHerald.Enums;
using TaiwanWeatherHerald.Interfaces;
using TaiwanWeatherHerald.Models;
using TaiwanWeatherHerald.Services.WeatherService.Models;

namespace TaiwanWeatherHerald.Services.WeatherService
{
    public class WeatherService : IWeatherService
    {
        /// <summary>
        /// 獲取旅遊地點天氣訊息
        /// </summary>
        /// <param name="tripType"></param>
        /// <returns></returns>
        public async Task<List<TripLocationWeatherInfo>> GetTripLocationWeatherInfosAsync(TripType tripType)
        {
            try
            {
                var weeklyForecastInfosTask = GetWeeklyForecastInfosAsync(tripType);
                var hourly72ForecastInfosTask = GetHourly72ForecastInfosAsync(tripType);
                var weatherMapInfoTask = GetWeatherMapInfoAsync();
                var weeklyForecastInfos = await weeklyForecastInfosTask;
                var hourly72ForecastInfos = await hourly72ForecastInfosTask;
                var weatherMapInfo = await weatherMapInfoTask;
                var locations = weeklyForecastInfos.Select(info => info.Location).Intersect(hourly72ForecastInfos.Select(info => info.Location));
                var tripLocationWeatherInfoTasks = locations.Select(location => Task.Run(async () =>
                                                            {
                                                                var weeklyForecastInfo = weeklyForecastInfos.Find(info => info.Location == location);
                                                                var hourly72ForecastInfo = hourly72ForecastInfos.Find(info => info.Location == location);
                                                                var assistantInfoTask = GetAssistantInfoAsync(weeklyForecastInfo.City);
                                                                return new TripLocationWeatherInfo
                                                                {
                                                                    Location = location,
                                                                    Latitude = weeklyForecastInfo.Latitude,
                                                                    Longitude = weeklyForecastInfo.Longitude,
                                                                    WeeklyForecastInfo = weeklyForecastInfo.WeatherForecastInfos,
                                                                    Hourly72ForecastInfo = hourly72ForecastInfo.WeatherForecastInfos,
                                                                    AssistantInfo = await assistantInfoTask,
                                                                    WeatherMapInfo = weatherMapInfo
                                                                };
                                                            }));
                return (await Task.WhenAll(tripLocationWeatherInfoTasks)).ToList();
            }
            catch (Exception)
            {
                return new List<TripLocationWeatherInfo>();
            }
        }

        #region 天氣預報

        private readonly Dictionary<TripType, string> _weeklyForecastDataIndexes = new Dictionary<TripType, string>
        {
            { TripType.Farm, "F-B0053-015" },
            { TripType.NationalPark, "F-B0053-039" },
            { TripType.NationalScenicArea,"F-B0053-045" },
            { TripType.NationalForestRecreationArea, "F-B0053-057" },
            { TripType.Reservoir, "F-B0053-063" }
        };

        private async Task<List<InternalForecastInfo>> GetWeeklyForecastInfosAsync(TripType tripType)
        {
            using (var httpClient = new HttpClient())
            {
                var dataIndex = _weeklyForecastDataIndexes[tripType];
                var dataUrl = $"https://opendata.cwa.gov.tw/fileapi/v1/opendataapi/{dataIndex}?Authorization={AuthorizationInfo.OpenWeatherData}&downloadType=WEB&format=XML";
                var dataString = await httpClient.GetStringAsync(dataUrl);
                var dataXml = XDocument.Parse(dataString);
                return SerializeWeeklyForecastInfos(dataXml);
            }
        }

        private static List<InternalForecastInfo> SerializeWeeklyForecastInfos(XDocument xDocument)
        {
            var locationCityDataSet = SerializeLocationCityTable(xDocument);
            var locationWeatherDataSet = xDocument.Deserialize<Models.DailyForecastInfo.CwaOpenData>();
            return locationWeatherDataSet.Dataset.Locations.Select(location => new InternalForecastInfo
                                                           {
                                                               City = locationCityDataSet[location.LocationName],
                                                               Location = location.LocationName,
                                                               Latitude = location.Latitude,
                                                               Longitude = location.Longitude,
                                                               WeatherForecastInfos = SerializeWeeklyForecastInfos(location.WeatherElements)
                                                           })
                                                           .ToList();
        }

        private static List<WeatherForecastInfo> SerializeWeeklyForecastInfos(List<Models.DailyForecastInfo.WeatherElement> weatherElements)
        {
            var forecastInfos = weatherElements[0].Times
                                                  .Select(info => new WeatherForecastInfo
                                                  {
                                                      DayNightType = GetDayNightType(info.StartTime),
                                                      DateTime = info.StartTime
                                                  })
                                                  .ToList();
            foreach (var weatherElement in weatherElements)
            {
                foreach (var timeWeatherElement in weatherElement.Times)
                {
                    var forecastInfo = forecastInfos.Find(info => info.DateTime == timeWeatherElement.StartTime);
                    if (forecastInfo != null)
                    {
                        switch (weatherElement.ElementName)
                        {
                            case "平均溫度":
                                forecastInfo.Temperature = timeWeatherElement.ElementValue.Temperature.Value;
                                break;
                            case "平均相對濕度":
                                forecastInfo.RelativeHumidity = timeWeatherElement.ElementValue.RelativeHumidity;
                                break;
                            case "風速":
                                var beaufortScaleMatch = Regex.Match(timeWeatherElement.ElementValue.BeaufortScale, @"\d+");
                                if (beaufortScaleMatch.Success)
                                {
                                    forecastInfo.WindBeaufortScale = int.Parse(beaufortScaleMatch.Value);
                                }
                                break;
                            case "風向":
                                if (timeWeatherElement.ElementValue.WindDirection.Contains("東") && timeWeatherElement.ElementValue.WindDirection.Contains("北"))
                                {
                                    forecastInfo.WindDirection = WindDirectionType.Northeast;
                                }
                                else if (timeWeatherElement.ElementValue.WindDirection.Contains("東") && timeWeatherElement.ElementValue.WindDirection.Contains("南"))
                                {
                                    forecastInfo.WindDirection = WindDirectionType.Southeast;
                                }
                                else if (timeWeatherElement.ElementValue.WindDirection.Contains("西") && timeWeatherElement.ElementValue.WindDirection.Contains("南"))
                                {
                                    forecastInfo.WindDirection = WindDirectionType.Southwest;
                                }
                                else if (timeWeatherElement.ElementValue.WindDirection.Contains("西") && timeWeatherElement.ElementValue.WindDirection.Contains("北"))
                                {
                                    forecastInfo.WindDirection = WindDirectionType.Northwest;
                                }
                                else if (timeWeatherElement.ElementValue.WindDirection.Contains("東"))
                                {
                                    forecastInfo.WindDirection = WindDirectionType.East;
                                }
                                else if (timeWeatherElement.ElementValue.WindDirection.Contains("南"))
                                {
                                    forecastInfo.WindDirection = WindDirectionType.South;
                                }
                                else if (timeWeatherElement.ElementValue.WindDirection.Contains("西"))
                                {
                                    forecastInfo.WindDirection = WindDirectionType.West;
                                }
                                else if (timeWeatherElement.ElementValue.WindDirection.Contains("北"))
                                {
                                    forecastInfo.WindDirection = WindDirectionType.North;
                                }
                                else
                                {
                                    forecastInfo.WindDirection = null;
                                }
                                break;
                            case "12小時降雨機率":
                                forecastInfo.ProbabilityOfPrecipitation = int.TryParse(timeWeatherElement.ElementValue.ProbabilityOfPrecipitation, out var value) ? value : null;
                                break;
                            case "天氣現象":
                                forecastInfo.Weather = GetWeatherTypeFromCode(timeWeatherElement.ElementValue.WeatherCode.Value);
                                forecastInfo.WeatherDescription = timeWeatherElement.ElementValue.Weather;
                                break;
                            case "紫外線指數":
                                forecastInfo.UVIndex = timeWeatherElement.ElementValue.UVIndex;
                                break;
                            case "天氣預報綜合描述":
                                forecastInfo.WeatherDetailDescription = timeWeatherElement.ElementValue.WeatherDescription.Replace("。", "。\n").TrimEnd();
                                break;
                        }
                    }
                }
            }
            return forecastInfos;
        }

        private readonly Dictionary<TripType, string> _hourly72ForecastDataIndexes = new Dictionary<TripType, string>
        {
            { TripType.Farm, "F-B0053-017" },
            { TripType.NationalPark, "F-B0053-041" },
            { TripType.NationalScenicArea,"F-B0053-047" },
            { TripType.NationalForestRecreationArea, "F-B0053-059" },
            { TripType.Reservoir, "F-B0053-065" }
        };

        public async Task<List<InternalForecastInfo>> GetHourly72ForecastInfosAsync(TripType tripType)
        {
            using (var httpClient = new HttpClient())
            {
                var dataIndex = _hourly72ForecastDataIndexes[tripType];
                var dataUrl = $"https://opendata.cwa.gov.tw/fileapi/v1/opendataapi/{dataIndex}?Authorization={AuthorizationInfo.OpenWeatherData}&downloadType=WEB&format=XML";
                var dataString = await httpClient.GetStringAsync(dataUrl);
                var dataXml = XDocument.Parse(dataString);
                return SerializeHourly72ForecastInfos(dataXml);
            }
        }

        private static List<InternalForecastInfo> SerializeHourly72ForecastInfos(XDocument xDocument)
        {
            var locationCityDataSet = SerializeLocationCityTable(xDocument);
            var locationWeatherDataSet = xDocument.Deserialize<Models.HourlyForecastInfo.CwaOpenData>();
            return locationWeatherDataSet.Dataset.Locations.Select(location => new InternalForecastInfo
                                                           {
                                                               City = locationCityDataSet[location.LocationName],
                                                               Location = location.LocationName,
                                                               Latitude = location.Latitude,
                                                               Longitude = location.Longitude,
                                                               WeatherForecastInfos = SerializeHourly72ForecastInfos(location.WeatherElements)
                                                           })
                                                           .ToList();
        }

        private static List<WeatherForecastInfo> SerializeHourly72ForecastInfos(List<Models.HourlyForecastInfo.WeatherElement> weatherElements)
        {
            var forecastInfos = weatherElements.First(info => info.ElementName == "溫度")
                                               .Times
                                               .Select(info => new WeatherForecastInfo
                                               {
                                                   DayNightType = GetDayNightType(info.DataTime.Value),
                                                   DateTime = info.DataTime.Value
                                               })
                                               .ToList();
            foreach (var weatherElement in weatherElements)
            {
                switch (weatherElement.ElementName)
                {
                    case "溫度":
                        foreach (var timeWeatherElement in weatherElement.Times)
                        {
                            var matchingForecastInfo = forecastInfos.Find(info => timeWeatherElement.DataTime.HasValue && timeWeatherElement.DataTime == info.DateTime);
                            if (matchingForecastInfo != null)
                            {
                                matchingForecastInfo.Temperature = timeWeatherElement.ElementValue.Temperature.Value;
                            }
                        }
                        break;
                    case "相對濕度":
                        foreach (var timeWeatherElement in weatherElement.Times)
                        {
                            var matchingForecastInfo = forecastInfos.Find(info => timeWeatherElement.DataTime.HasValue && timeWeatherElement.DataTime == info.DateTime);
                            if (matchingForecastInfo != null)
                            {
                                matchingForecastInfo.RelativeHumidity = timeWeatherElement.ElementValue.RelativeHumidity;
                            }
                        }
                        break;
                    case "風速":
                        foreach (var timeWeatherElement in weatherElement.Times)
                        {
                            var matchingForecastInfos = forecastInfos.FindAll(info => timeWeatherElement.DataTime.HasValue && timeWeatherElement.DataTime <= info.DateTime);
                            foreach (var matchingForecastInfo in matchingForecastInfos)
                            {
                                var beaufortScaleMatch = Regex.Match(timeWeatherElement.ElementValue.BeaufortScale, @"\d+");
                                if (beaufortScaleMatch.Success)
                                {
                                    matchingForecastInfo.WindBeaufortScale = int.Parse(beaufortScaleMatch.Value);
                                }
                            }
                        }
                        break;
                    case "風向":
                        foreach (var timeWeatherElement in weatherElement.Times)
                        {
                            var matchingForecastInfos = forecastInfos.FindAll(info => timeWeatherElement.DataTime.HasValue && timeWeatherElement.DataTime <= info.DateTime);
                            foreach (var matchingForecastInfo in matchingForecastInfos)
                            {
                                if (timeWeatherElement.ElementValue.WindDirection.Contains("東") && timeWeatherElement.ElementValue.WindDirection.Contains("北"))
                                {
                                    matchingForecastInfo.WindDirection = WindDirectionType.Northeast;
                                }
                                else if (timeWeatherElement.ElementValue.WindDirection.Contains("東") && timeWeatherElement.ElementValue.WindDirection.Contains("南"))
                                {
                                    matchingForecastInfo.WindDirection = WindDirectionType.Southeast;
                                }
                                else if (timeWeatherElement.ElementValue.WindDirection.Contains("西") && timeWeatherElement.ElementValue.WindDirection.Contains("南"))
                                {
                                    matchingForecastInfo.WindDirection = WindDirectionType.Southwest;
                                }
                                else if (timeWeatherElement.ElementValue.WindDirection.Contains("西") && timeWeatherElement.ElementValue.WindDirection.Contains("北"))
                                {
                                    matchingForecastInfo.WindDirection = WindDirectionType.Northwest;
                                }
                                else if (timeWeatherElement.ElementValue.WindDirection.Contains("東"))
                                {
                                    matchingForecastInfo.WindDirection = WindDirectionType.East;
                                }
                                else if (timeWeatherElement.ElementValue.WindDirection.Contains("南"))
                                {
                                    matchingForecastInfo.WindDirection = WindDirectionType.South;
                                }
                                else if (timeWeatherElement.ElementValue.WindDirection.Contains("西"))
                                {
                                    matchingForecastInfo.WindDirection = WindDirectionType.West;
                                }
                                else if (timeWeatherElement.ElementValue.WindDirection.Contains("北"))
                                {
                                    matchingForecastInfo.WindDirection = WindDirectionType.North;
                                }
                                else
                                {
                                    matchingForecastInfo.WindDirection = null;
                                }
                            }
                        }
                        break;
                    case "3小時降雨機率":
                        foreach (var timeWeatherElement in weatherElement.Times)
                        {
                            var matchingForecastInfos = forecastInfos.FindAll(info => timeWeatherElement.StartTime.HasValue && timeWeatherElement.EndTime.HasValue && timeWeatherElement.StartTime.Value <= info.DateTime && info.DateTime < timeWeatherElement.EndTime.Value);
                            foreach (var matchingForecastInfo in matchingForecastInfos)
                            {
                                matchingForecastInfo.ProbabilityOfPrecipitation = int.TryParse(timeWeatherElement.ElementValue.ProbabilityOfPrecipitation, out var value) ? value : null;
                            }
                        }
                        break;
                    case "天氣現象":
                        foreach (var timeWeatherElement in weatherElement.Times)
                        {
                            var matchingForecastInfos = forecastInfos.FindAll(info => timeWeatherElement.StartTime.HasValue && timeWeatherElement.EndTime.HasValue && timeWeatherElement.StartTime.Value <= info.DateTime && info.DateTime < timeWeatherElement.EndTime.Value);
                            foreach (var matchingForecastInfo in matchingForecastInfos)
                            {
                                matchingForecastInfo.Weather = GetWeatherTypeFromCode(timeWeatherElement.ElementValue.WeatherCode.Value);
                                matchingForecastInfo.WeatherDescription = timeWeatherElement.ElementValue.Weather;
                            }
                        }
                        break;
                    case "天氣預報綜合描述":
                        foreach (var timeWeatherElement in weatherElement.Times)
                        {
                            var matchingForecastInfos = forecastInfos.FindAll(info => timeWeatherElement.StartTime.HasValue && timeWeatherElement.EndTime.HasValue && timeWeatherElement.StartTime.Value <= info.DateTime && info.DateTime < timeWeatherElement.EndTime.Value);
                            foreach (var matchingForecastInfo in matchingForecastInfos)
                            {
                                matchingForecastInfo.WeatherDetailDescription = timeWeatherElement.ElementValue.WeatherDescription.Replace("。", "。\n").TrimEnd();
                            }
                        }
                        break;
                }
            }
            return forecastInfos;
        }

        private static Dictionary<string, string> SerializeLocationCityTable(XDocument dataXml)
        {
            var locationCityTable = new Dictionary<string, string>();

            // 取得命名空間
            var namespaceName = dataXml.Root.GetDefaultNamespace();

            // 獲取 Locations 元素
            var locationsElements = dataXml.Descendants(namespaceName + "Locations").First().Elements();

            // 遍歷 Locations 元素
            string cityName = string.Empty;
            foreach (var element in locationsElements)
            {
                switch (element.Name.LocalName)
                {
                    case "LocationsName":
                        cityName = element.Value.Length >= 3 ? element.Value[..3] : element.Value;
                        break;
                    case "Location":
                        if (!string.IsNullOrEmpty(cityName))
                        {
                            var locationName = element.Element(namespaceName + "LocationName")?.Value;
                            if (!string.IsNullOrEmpty(locationName))
                            {
                                locationCityTable[locationName] = cityName;
                            }
                        }
                        break;
                }
            }

            return locationCityTable;
        }

        private static WeatherType GetWeatherTypeFromCode(int code) => code switch
        {
            1 => WeatherType.Clear,
            2 or 3 or 4 or 5 or 6 or 7 => WeatherType.Cloudy,
            8 or 9 or 10 or 11 or 12 or 13 or 14 or 19 or 20 or 29 or 30 or 31 or 32 or 38 or 39 => WeatherType.Rainy,
            15 or 16 or 17 or 18 or 21 or 22 or 33 or 34 or 35 or 36 or 41 => WeatherType.Stormy,
            24 or 25 or 26 or 27 or 28 => WeatherType.Foggy,
            23 or 37 or 42 => WeatherType.Snowy
        };

        private static DayNightType GetDayNightType(DateTime dateTime)
        {
            return (dateTime.Hour >= 6 && dateTime.Hour < 18) ? DayNightType.Day : DayNightType.Night;
        }

        #endregion

        #region 天氣小幫手

        private readonly Dictionary<string, string> _assistantDataIndexes = new Dictionary<string, string>
        {
            { "基隆市", "F-C0032-011" },
            { "臺北市", "F-C0032-009" },
            { "新北市", "F-C0032-010" },
            { "桃園市", "F-C0032-022" },
            { "新竹市", "F-C0032-024" },
            { "新竹縣", "F-C0032-023" },
            { "苗栗縣", "F-C0032-020" },
            { "臺中市", "F-C0032-021" },
            { "彰化縣", "F-C0032-028" },
            { "南投縣", "F-C0032-026" },
            { "雲林縣", "F-C0032-029" },
            { "嘉義市", "F-C0032-019" },
            { "嘉義縣", "F-C0032-018" },
            { "臺南市", "F-C0032-016" },
            { "高雄市", "F-C0032-017" },
            { "屏東縣", "F-C0032-025" },
            { "宜蘭縣", "F-C0032-013" },
            { "花蓮縣", "F-C0032-012" },
            { "臺東縣", "F-C0032-027" },
            { "澎湖縣", "F-C0032-015" },
            { "金門縣", "F-C0032-014" },
            { "連江縣", "F-C0032-030" }
        };

        private async Task<WeatherAssistantInfo> GetAssistantInfoAsync(string city)
        {
            using (var httpClient = new HttpClient())
            {
                var dataIndex = _assistantDataIndexes[city];
                var dataUrl = $"https://opendata.cwa.gov.tw/fileapi/v1/opendataapi/{dataIndex}?Authorization={AuthorizationInfo.OpenWeatherData}&downloadType=WEB&format=XML";
                var dataString = await httpClient.GetStringAsync(dataUrl);
                var dataXml = XDocument.Parse(dataString);
                var dataNamespace = dataXml.Root.GetDefaultNamespace();
                //獲取所有parameterValue
                var parameterValues = dataXml.Descendants(dataNamespace + "parameterValue")
                                             .Select(parameterValue => parameterValue.Value)
                                             .ToList();
                //第一個parameterValue是標題
                var title = parameterValues[0];
                //其餘的parameterValue是內文
                var content = string.Join("\n", parameterValues.Skip(1));
                return new WeatherAssistantInfo
                {
                    Title = title,
                    Content = content
                };
            }
        }

        #endregion

        #region 天氣圖資

        private async Task<WeatherMapInfo> GetWeatherMapInfoAsync()
        {
            //紅外線色調強化衛星雲圖-台灣
            var satelliteCloudMapUrlTask = GetMapUrlAsync($"https://opendata.cwa.gov.tw/fileapi/v1/opendataapi/O-B0030-003?Authorization={AuthorizationInfo.OpenWeatherData}&downloadType=WEB&format=XML");
            //雷達整合回波圖-臺灣-臺灣(鄰近區域)_無地形
            var rainRadarMapUrlTask = GetMapUrlAsync($"https://opendata.cwa.gov.tw/fileapi/v1/opendataapi/O-A0058-003?Authorization={AuthorizationInfo.OpenWeatherData}&downloadType=WEB&format=XML");
            //日累積雨量圖資料-小間距日累積雨量圖資料
            var accumulatedRainfallMapUrlTask = GetMapUrlAsync($"https://opendata.cwa.gov.tw/fileapi/v1/opendataapi/O-A0040-002?Authorization={AuthorizationInfo.OpenWeatherData}&downloadType=WEB&format=XML");
            //溫度分布圖
            var temperatureDistributionMapUrlTask = GetMapUrlAsync($"https://opendata.cwa.gov.tw/fileapi/v1/opendataapi/O-A0038-001?Authorization={AuthorizationInfo.OpenWeatherData}&downloadType=WEB&format=XML");
            return new WeatherMapInfo
            {
                SatelliteCloudMapUrl = await satelliteCloudMapUrlTask,
                RainRadarMapUrl = await rainRadarMapUrlTask,
                AccumulatedRainfallMapUrl = await accumulatedRainfallMapUrlTask,
                TemperatureDistributionMapUrl = await temperatureDistributionMapUrlTask
            };
        }

        private static async Task<string> GetMapUrlAsync(string dataUrl)
        {
            using (var httpClient = new HttpClient())
            {
                var dataString = await httpClient.GetStringAsync(dataUrl);
                var dataXml = XDocument.Parse(dataString);
                var dataNamespace = dataXml.Root.GetDefaultNamespace();
                var mapUrl = dataXml.Descendants(dataNamespace + "ProductURL").First().Value;
                return mapUrl;
            }
        }

        #endregion
    }

    public static class XDocumentExtensions
    {
        public static T Deserialize<T>(this XDocument xDocument)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = xDocument.CreateReader())
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}

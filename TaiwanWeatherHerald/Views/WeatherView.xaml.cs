using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TaiwanWeatherHerald.Enums;
using TaiwanWeatherHerald.Interfaces;
using TaiwanWeatherHerald.Models;
using TaiwanWeatherHerald.Services;
using TaiwanWeatherHerald.Services.WeatherService;
using TaiwanWeatherHerald.UserControls;

namespace TaiwanWeatherHerald.Views
{
    public partial class WeatherView : UserControl
    {
        private readonly IWeatherService _weatherService = new WeatherService();
        private readonly ICCTVLiveVideoService _cctvLiveVideoService = new CCTVLiveVideoService();
        private List<TripLocationWeatherInfo> _tripLocationWeatherInfos;
        private string _selectedTripLocation;
        private WeatherInfoType? _selectedTripWeatherInfoType;

        public WeatherView()
        {
            InitializeComponent();
            InitializeTripTypeComboBox();
            InitializeTripLocationComboBox();
            InitializeTripWeatherInfoTypeComboBox();
        }

        private void InitializeTripTypeComboBox()
        {
            _tripLocationWeatherInfos = null;

            TripTypeComboBox.SelectionChanged -= TripTypeComboBox_SelectionChanged;

            var comboBoxItemObjects = new List<(TripType TripType, string DisplayName, bool IsSelected, bool IsEnabled)>
            {
                (default, "選擇旅遊地點", true, false),
                (TripType.Farm, "休閒農場", false, true),
                (TripType.NationalPark, "國家公園", false, true),
                (TripType.NationalScenicArea, "國家風景區", false, true),
                (TripType.NationalForestRecreationArea, "國家森林遊樂區", false, true),
                (TripType.Reservoir, "水庫", false, true)
            };
            TripTypeComboBox.Items.Clear();
            foreach (var comboBoxItemObject in comboBoxItemObjects)
            {
                TripTypeComboBox.Items.Add(new ComboBoxItem
                {
                    Tag = comboBoxItemObject.TripType,
                    Content = comboBoxItemObject.DisplayName,
                    IsSelected = comboBoxItemObject.IsSelected,
                    IsEnabled = comboBoxItemObject.IsEnabled
                });
            }

            TripTypeComboBox.SelectionChanged += TripTypeComboBox_SelectionChanged;
        }

        private void InitializeTripLocationComboBox()
        {
            _selectedTripLocation = null;

            TripLocationComboBox.Items.Clear();
            TripLocationComboBox.Items.Add(new ComboBoxItem { Content = "選擇旅遊地點", IsSelected = true, IsEnabled = false });
        }

        private void InitializeTripLocationComboBox(List<string> locations)
        {
            TripLocationComboBox.SelectionChanged -= TripLocationComboBox_SelectionChanged;

            InitializeTripLocationComboBox();
            foreach (var location in locations)
            {
                TripLocationComboBox.Items.Add(new ComboBoxItem { Content = location, IsSelected = false, IsEnabled = true });
            }

            TripLocationComboBox.SelectionChanged += TripLocationComboBox_SelectionChanged;
        }

        private void InitializeTripWeatherInfoTypeComboBox()
        {
            _selectedTripWeatherInfoType = null;

            TripWeatherInfoTypeComboBox.SelectionChanged -= TripWeatherInfoTypeComboBox_SelectionChanged;

            var comboBoxItemObjects = new List<(WeatherInfoType WeatherInfoType, string DisplayName, bool IsSelected, bool IsEnabled)>
            {
                (default, "選擇旅遊天氣訊息類型", true, false),
                (WeatherInfoType.Hourly72Forecast, "72小時天氣預報", false, true),
                (WeatherInfoType.WeeklyForecast, "一週天氣預報", false, true),
                (WeatherInfoType.WeatherAssistant, "天氣小幫手", false, true),
                (WeatherInfoType.WeatherMap, "天氣圖資", false, true),
                (WeatherInfoType.CCTVLiveVideo, "監視器即時影像", false, true)
            };
            TripWeatherInfoTypeComboBox.Items.Clear();
            foreach (var comboBoxItemObject in comboBoxItemObjects)
            {
                TripWeatherInfoTypeComboBox.Items.Add(new ComboBoxItem
                {
                    Tag = comboBoxItemObject.WeatherInfoType,
                    Content = comboBoxItemObject.DisplayName,
                    IsSelected = comboBoxItemObject.IsSelected,
                    IsEnabled = comboBoxItemObject.IsEnabled
                });
            }

            TripWeatherInfoTypeComboBox.SelectionChanged += TripWeatherInfoTypeComboBox_SelectionChanged;
        }

        private async void TripTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TripTypeComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is TripType selectedTripType)
            {
                InitializeTripLocationComboBox();

                _tripLocationWeatherInfos = await _weatherService.GetTripLocationWeatherInfosAsync(selectedTripType);

                InitializeTripLocationComboBox(_tripLocationWeatherInfos.Select(info => info.Location).ToList());
            }
            else
            {
                _tripLocationWeatherInfos = null;
            }
        }

        private async void TripLocationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TripLocationComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                _selectedTripLocation = selectedItem.Content.ToString();
                await ShowAsync();
            }
            else
            {
                _selectedTripLocation = null;
            }
        }

        private async void TripWeatherInfoTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TripWeatherInfoTypeComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is WeatherInfoType selectedTripWeatherInfoType)
            {
                _selectedTripWeatherInfoType = selectedTripWeatherInfoType;
                await ShowAsync();
            }
            else
            {
                _selectedTripWeatherInfoType = null;
            }
        }

        private async Task ShowAsync()
        {
            if (_tripLocationWeatherInfos != null && _tripLocationWeatherInfos.Count > 0 && !string.IsNullOrEmpty(_selectedTripLocation) && _selectedTripWeatherInfoType.HasValue)
            {
                var tripLocationWeatherInfo = _tripLocationWeatherInfos.FirstOrDefault(info => info.Location == _selectedTripLocation);
                switch (_selectedTripWeatherInfoType)
                {
                    case WeatherInfoType.Hourly72Forecast:
                        ShowHourly72Forecast(tripLocationWeatherInfo);
                        break;
                    case WeatherInfoType.WeeklyForecast:
                        ShowWeeklyForecast(tripLocationWeatherInfo);
                        break;
                    case WeatherInfoType.WeatherAssistant:
                        ShowWeatherAssistant(tripLocationWeatherInfo);
                        break;
                    case WeatherInfoType.WeatherMap:
                        ShowWeatherMap(tripLocationWeatherInfo);
                        break;
                    case WeatherInfoType.CCTVLiveVideo:
                        await ShowCCTVLiveVideoAsync(tripLocationWeatherInfo);
                        break;
                }
            }
        }

        private void ShowHourly72Forecast(TripLocationWeatherInfo info)
        {
            DisplayArea.Children.Clear();
            if (info != null && info.Hourly72ForecastInfo != null && info.Hourly72ForecastInfo.Count > 0)
            {
                var globalMinTemperature = info.Hourly72ForecastInfo.Min(hourlyForecastInfo => Convert.ToInt32(hourlyForecastInfo.Temperature));
                var globalMaxTemperature = info.Hourly72ForecastInfo.Max(hourlyForecastInfo => Convert.ToInt32(hourlyForecastInfo.Temperature));
                foreach (var hourlyForecastInfo in info.Hourly72ForecastInfo)
                {
                    DisplayArea.Children.Add(new ForecastCard()
                    {
                        ForecastType = ForecastType.Hourly,
                        DayNightType = hourlyForecastInfo.DayNightType,
                        Date = hourlyForecastInfo.DateTime,
                        WeatherDetailDescription = hourlyForecastInfo.WeatherDetailDescription,
                        Weather = hourlyForecastInfo.Weather,
                        ProbabilityOfPrecipitation = hourlyForecastInfo.ProbabilityOfPrecipitation,
                        UVIndex = hourlyForecastInfo.UVIndex,
                        RelativeHumidity = hourlyForecastInfo.RelativeHumidity,
                        WindBeaufortScale = hourlyForecastInfo.WindBeaufortScale,
                        WindDirection = hourlyForecastInfo.WindDirection,
                        GlobalMinTemperature = globalMinTemperature,
                        GlobalMaxTemperature = globalMaxTemperature,
                        Temperature = hourlyForecastInfo.Temperature
                    });
                }
            }
        }

        private void ShowWeeklyForecast(TripLocationWeatherInfo info)
        {
            DisplayArea.Children.Clear();
            if (info != null && info.WeeklyForecastInfo != null && info.WeeklyForecastInfo.Count > 0)
            {
                var globalMinTemperature = info.WeeklyForecastInfo.Min(dailyForecastInfo => Convert.ToInt32(dailyForecastInfo.Temperature));
                var globalMaxTemperature = info.WeeklyForecastInfo.Max(dailyForecastInfo => Convert.ToInt32(dailyForecastInfo.Temperature));
                foreach (var dailyForecastInfo in info.WeeklyForecastInfo)
                {
                    DisplayArea.Children.Add(new ForecastCard()
                    {
                        ForecastType = ForecastType.Daily,
                        DayNightType = dailyForecastInfo.DayNightType,
                        Date = dailyForecastInfo.DateTime,
                        WeatherDetailDescription = dailyForecastInfo.WeatherDetailDescription,
                        Weather = dailyForecastInfo.Weather,
                        ProbabilityOfPrecipitation = dailyForecastInfo.ProbabilityOfPrecipitation,
                        UVIndex = dailyForecastInfo.UVIndex,
                        RelativeHumidity = dailyForecastInfo.RelativeHumidity,
                        WindBeaufortScale = dailyForecastInfo.WindBeaufortScale,
                        WindDirection = dailyForecastInfo.WindDirection,
                        GlobalMinTemperature = globalMinTemperature,
                        GlobalMaxTemperature = globalMaxTemperature,
                        Temperature = dailyForecastInfo.Temperature
                    });
                }
            }
        }

        private void ShowWeatherAssistant(TripLocationWeatherInfo info)
        {
            DisplayArea.Children.Clear();
            if (info != null && info.AssistantInfo != null)
            {
                DisplayArea.Children.Add(new AssistantCard()
                {
                    Title = info.AssistantInfo.Title,
                    Content = info.AssistantInfo.Content
                });
            }
        }

        private void ShowWeatherMap(TripLocationWeatherInfo info)
        {
            DisplayArea.Children.Clear();
            if (info != null && info.WeatherMapInfo != null)
            {
                DisplayArea.Children.Add(new WeatherMapCard
                {
                    Images = [ new BitmapImage(new Uri(info.WeatherMapInfo.SatelliteCloudMapUrl, UriKind.RelativeOrAbsolute)),
                               new BitmapImage(new Uri(info.WeatherMapInfo.RainRadarMapUrl, UriKind.RelativeOrAbsolute)),
                               new BitmapImage(new Uri(info.WeatherMapInfo.AccumulatedRainfallMapUrl, UriKind.RelativeOrAbsolute)),
                               new BitmapImage(new Uri(info.WeatherMapInfo.TemperatureDistributionMapUrl, UriKind.RelativeOrAbsolute))]
                });
            }
        }

        private async Task ShowCCTVLiveVideoAsync(TripLocationWeatherInfo info)
        {
            DisplayArea.Children.Clear();
            if (info != null)
            {
                DisplayArea.Children.Add(new CCTVMapCard
                {
                    LocationName = info.Location,
                    LocationLongitude = info.Longitude,
                    LocationLatitude = info.Latitude,
                    CCTVInfos = await _cctvLiveVideoService.GetCCTVLiveVideoInfos(info.Longitude, info.Latitude)
                });
            }
        }
    }
}

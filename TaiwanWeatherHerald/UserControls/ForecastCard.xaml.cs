using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TaiwanWeatherHerald.Enums;

namespace TaiwanWeatherHerald.UserControls
{
    public partial class ForecastCard : UserControl
    {
        public ForecastCard()
        {
            InitializeComponent();
        }

        public ForecastType ForecastType
        {
            get
            {
                return (ForecastType)GetValue(ForecastTypeProperty);
            }
            set
            {
                SetValue(ForecastTypeProperty, value);
                UpdateDateString();
            }
        }
        public static readonly DependencyProperty ForecastTypeProperty = DependencyProperty.Register("ForecastType", typeof(ForecastType), typeof(ForecastCard));

        public DayNightType DayNightType
        {
            get
            {
                return (DayNightType)GetValue(DayNightTypeProperty);
            }
            set
            {
                SetValue(DayNightTypeProperty, value);
                UpdateDateString();
                UpdateWeatherImage();
            }
        }
        public static readonly DependencyProperty DayNightTypeProperty = DependencyProperty.Register("DayNightType", typeof(DayNightType), typeof(ForecastCard));

        public DateTime Date
        {
            get
            {
                return (DateTime)GetValue(DateProperty);
            }
            set
            {
                SetValue(DateProperty, value);
                UpdateDateString();
            }
        }
        public static readonly DependencyProperty DateProperty = DependencyProperty.Register("Date", typeof(DateTime), typeof(ForecastCard));

        public string DateString
        {
            get
            {
                return (string)GetValue(DateStringProperty);
            }
            set
            {
                SetValue(DateStringProperty, value);
            }
        }
        public static readonly DependencyProperty DateStringProperty = DependencyProperty.Register("DateString", typeof(string), typeof(ForecastCard));

        public string WeatherDetailDescription
        {
            get
            {
                return (string)GetValue(WeatherDetailDescriptionProperty);
            }
            set
            {
                SetValue(WeatherDetailDescriptionProperty, value);
            }
        }
        public static readonly DependencyProperty WeatherDetailDescriptionProperty = DependencyProperty.Register("WeatherDetailDescription", typeof(string), typeof(ForecastCard));

        public WeatherType Weather
        {
            get
            {
                return (WeatherType)GetValue(WeatherProperty);
            }
            set
            {
                SetValue(WeatherProperty, value);
                UpdateWeatherImage();
            }
        }
        public static readonly DependencyProperty WeatherProperty = DependencyProperty.Register("Weather", typeof(WeatherType), typeof(ForecastCard));

        public ImageSource WeatherImage
        {
            get
            {
                return (ImageSource)GetValue(WeatherImageProperty);
            }
            set
            {
                SetValue(WeatherImageProperty, value);
            }
        }
        public static readonly DependencyProperty WeatherImageProperty = DependencyProperty.Register("WeatherImage", typeof(ImageSource), typeof(ForecastCard));

        public int? ProbabilityOfPrecipitation
        {
            get
            {
                return (int?)GetValue(ProbabilityOfPrecipitationProperty);
            }
            set
            {
                SetValue(ProbabilityOfPrecipitationProperty, value);
                UpdateProbabilityOfPrecipitationImage();
            }
        }
        public static readonly DependencyProperty ProbabilityOfPrecipitationProperty = DependencyProperty.Register("ProbabilityOfPrecipitation", typeof(int?), typeof(ForecastCard));

        public ImageSource ProbabilityOfPrecipitationImage
        {
            get
            {
                return (ImageSource)GetValue(ProbabilityOfPrecipitationImageProperty);
            }
            set
            {
                SetValue(ProbabilityOfPrecipitationImageProperty, value);
            }
        }
        public static readonly DependencyProperty ProbabilityOfPrecipitationImageProperty = DependencyProperty.Register("ProbabilityOfPrecipitationImage", typeof(ImageSource), typeof(ForecastCard));

        public int? UVIndex
        {
            get
            {
                return (int?)GetValue(UVIndexProperty);
            }
            set
            {
                SetValue(UVIndexProperty, value);
                UpdateUVIndexImage();
            }
        }
        public static readonly DependencyProperty UVIndexProperty = DependencyProperty.Register("UVIndex", typeof(int?), typeof(ForecastCard));

        public ImageSource UVIndexImage
        {
            get
            {
                return (ImageSource)GetValue(UVIndexImageProperty);
            }
            set
            {
                SetValue(UVIndexImageProperty, value);
            }
        }
        public static readonly DependencyProperty UVIndexImageProperty = DependencyProperty.Register("UVIndexImage", typeof(ImageSource), typeof(ForecastCard));

        public int? RelativeHumidity
        {
            get 
            {
                return (int?)GetValue(RelativeHumidityProperty);
            }
            set
            {
                SetValue(RelativeHumidityProperty, value);
                UpdateRelativeHumidityImage();
            }
        }
        public static readonly DependencyProperty RelativeHumidityProperty = DependencyProperty.Register("RelativeHumidity", typeof(int?), typeof(ForecastCard));

        public ImageSource RelativeHumidityImage
        {
            get
            {
                return (ImageSource)GetValue(RelativeHumidityImageProperty);
            }
            set
            { 
                SetValue(RelativeHumidityImageProperty, value);
            }
        }
        public static readonly DependencyProperty RelativeHumidityImageProperty = DependencyProperty.Register("RelativeHumidityImage", typeof(ImageSource), typeof(ForecastCard));

        public int? WindBeaufortScale
        {
            get
            {
                return (int?)GetValue(WindBeaufortScaleProperty);
            }
            set
            {
                SetValue(WindBeaufortScaleProperty, value);
                UpdateWindBeaufortScaleAndDirectionImages();
            }
        }
        public static readonly DependencyProperty WindBeaufortScaleProperty = DependencyProperty.Register("WindBeaufortScale", typeof(int?), typeof(ForecastCard));

        public ImageSource WindBeaufortScaleImage
        {
            get
            {
                return (ImageSource)GetValue(WindBeaufortScaleImageProperty);
            }
            set
            {
                SetValue(WindBeaufortScaleImageProperty, value);
            }
        }
        public static readonly DependencyProperty WindBeaufortScaleImageProperty = DependencyProperty.Register("WindBeaufortScaleImage", typeof(ImageSource), typeof(ForecastCard));

        public WindDirectionType? WindDirection
        {
            get
            {
                return (WindDirectionType?)GetValue(WindDirectionProperty);
            }
            set
            {
                SetValue(WindDirectionProperty, value);
                UpdateWindBeaufortScaleAndDirectionImages();
            }
        }
        public static readonly DependencyProperty WindDirectionProperty = DependencyProperty.Register("WindDirection", typeof(WindDirectionType?), typeof(ForecastCard));

        public ImageSource WindDirectionImage
        {
            get
            {
                return (ImageSource)GetValue(WindDirectionImageProperty);
            }
            set
            {
                SetValue(WindDirectionImageProperty, value);
            }
        }
        public static readonly DependencyProperty WindDirectionImageProperty = DependencyProperty.Register("WindDirectionImage", typeof(ImageSource), typeof(ForecastCard));

        public int GlobalMinTemperature
        {
            get
            {
                return (int)GetValue(GlobalMinTemperatureProperty);
            }
            set
            {
                SetValue(GlobalMinTemperatureProperty, value);
                UpdateTemperatureScaleMargin();
            }
        }
        public static readonly DependencyProperty GlobalMinTemperatureProperty = DependencyProperty.Register("GlobalMinTemperature", typeof(int), typeof(ForecastCard));

        public int GlobalMaxTemperature
        {
            get
            {
                return (int)GetValue(GlobalMaxTemperatureProperty);
            }
            set
            {
                SetValue(GlobalMaxTemperatureProperty, value);
                UpdateTemperatureScaleMargin();
            }
        }
        public static readonly DependencyProperty GlobalMaxTemperatureProperty = DependencyProperty.Register("GlobalMaxTemperature", typeof(int), typeof(ForecastCard));

        public int Temperature
        {
            get
            {
                return (int)GetValue(TemperatureProperty);
            }
            set
            {
                SetValue(TemperatureProperty, value);
                UpdateTemperatureScaleMargin();
            }
        }
        public static readonly DependencyProperty TemperatureProperty = DependencyProperty.Register("Temperature", typeof(int), typeof(ForecastCard));

        public int TemperatureScaleTopMargin
        {
            get
            {
                return (int)GetValue(TemperatureScaleTopMarginProperty);
            }
            private set
            {
                SetValue(TemperatureScaleTopMarginProperty, value);
            }
        }
        public static readonly DependencyProperty TemperatureScaleTopMarginProperty = DependencyProperty.Register("TemperatureScaleTopMargin", typeof(int), typeof(ForecastCard));

        #region UpdateProperty

        private void UpdateDateString()
        {
            switch (ForecastType)
            {
                case ForecastType.Hourly:
                    DateString = $"{Date:MM/dd} ({Date:ddd})\n{Date:HH:mm}";
                    break;
                case ForecastType.Daily:
                    DateString = $"{Date:MM/dd} ({Date:ddd})\n{(DayNightType == DayNightType.Day ? "白天" : "夜晚")}";
                    break;
            }
        }

        private void UpdateWeatherImage()
        {
            WeatherImage = (DayNightType == DayNightType.Day)
                         ? new BitmapImage(new Uri(@$"/Images/Day/{Weather}.png", UriKind.Relative))
                         : new BitmapImage(new Uri(@$"/Images/Night/{Weather}.png", UriKind.Relative));
        }

        private void UpdateProbabilityOfPrecipitationImage()
        {
            if (ProbabilityOfPrecipitation.HasValue)
            {
                ProbabilityOfPrecipitationImage = new BitmapImage(new Uri(@$"/Images/ProbabilityOfPrecipitation/{ProbabilityOfPrecipitation.Value}.png", UriKind.Relative));
            }
            else
            {
                ProbabilityOfPrecipitationImage = new BitmapImage(new Uri(@$"/Images/ProbabilityOfPrecipitation/NoData.png", UriKind.Relative));
            }
        }

        private void UpdateUVIndexImage()
        {
            if (UVIndex.HasValue)
            {
                UVIndexImage = new BitmapImage(new Uri(@$"/Images/UVIndex/{UVIndex.Value}.png", UriKind.Relative));
            }
            else
            {
                UVIndexImage = new BitmapImage(new Uri(@$"/Images/UVIndex/NoData.png", UriKind.Relative));
            }
        }

        private void UpdateRelativeHumidityImage()
        {
            if (RelativeHumidity.HasValue)
            {
                RelativeHumidityImage = new BitmapImage(new Uri(@$"/Images/RelativeHumidity/{RelativeHumidity.Value}.png", UriKind.Relative));
            }
            else
            {
                RelativeHumidityImage = new BitmapImage(new Uri(@$"/Images/RelativeHumidity/NoData.png", UriKind.Relative));
            }
        }

        private void UpdateWindBeaufortScaleAndDirectionImages()
        {
            if (WindBeaufortScale.HasValue && WindDirection.HasValue)
            {
                WindBeaufortScaleImage = new BitmapImage(new Uri(@$"/Images/WindBeaufort/{WindBeaufortScale.Value}.png", UriKind.Relative));
                WindDirectionImage = new BitmapImage(new Uri(@$"/Images/WindDirection/{WindDirection.Value}.png", UriKind.Relative));
            }
            else
            {
                WindBeaufortScaleImage = new BitmapImage(new Uri(@$"/Images/WindBeaufort/NoData.png", UriKind.Relative));
                WindDirectionImage = new BitmapImage(new Uri(@$"/Images/WindDirection/{WindDirectionType.East}.png", UriKind.Relative));
            }
        }

        private void UpdateTemperatureScaleMargin()
        {
            TemperatureScaleTopMargin = (int)((1 - (double)(Temperature - GlobalMinTemperature) / (GlobalMaxTemperature - GlobalMinTemperature)) * 60);
        }

        #endregion
    }
}

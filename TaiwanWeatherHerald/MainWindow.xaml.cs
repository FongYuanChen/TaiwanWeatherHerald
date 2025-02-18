using System.Windows;
using TaiwanWeatherHerald.Models;

namespace TaiwanWeatherHerald
{
    public partial class MainWindow : Window
    {
        private Views.WeatherView _weatherView;
        private Views.SettingsView _settingsView;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMainContent();
        }

        private void InitializeMainContent()
        {
            if (string.IsNullOrEmpty(AuthorizationInfo.OpenWeatherData))
            {
                _settingsView ??= new Views.SettingsView();
                MainContent.Content = _settingsView;
            }
            else
            {
                _weatherView ??= new Views.WeatherView();
                MainContent.Content = _weatherView;
            }
        }

        private void Weather_Click(object sender, RoutedEventArgs e)
        {
            _weatherView ??= new Views.WeatherView();
            MainContent.Content = _weatherView;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            _settingsView ??= new Views.SettingsView();
            MainContent.Content = _settingsView;
        }
    }
}
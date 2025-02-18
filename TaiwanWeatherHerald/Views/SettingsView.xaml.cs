using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using TaiwanWeatherHerald.Models;

namespace TaiwanWeatherHerald.Views
{
    public partial class SettingsView : UserControl
    {
        private bool _isPasswordVisible = false;

        public SettingsView()
        {
            InitializeComponent();

            ApiKeyPasswordBox.Password = AuthorizationInfo.OpenWeatherData;
            ApiKeyTextBox.Text = AuthorizationInfo.OpenWeatherData;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // 使用預設瀏覽器打開連結
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri, // 目標網址
                UseShellExecute = true        // 使用 Shell 打開
            });

            e.Handled = true; // 標記事件已處理
        }

        private void Step1Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Step1Popup.IsOpen = true;
        }

        private void Step1Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Step1Popup.IsOpen = false;
        }

        private void Step2Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Step2Popup.IsOpen = true;
        }

        private void Step2Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Step2Popup.IsOpen = false;
        }

        private void ApiKeyPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ApiKeyTextBox.Text = ApiKeyPasswordBox.Password;
            AuthorizationInfo.OpenWeatherData = ApiKeyPasswordBox.Password;
        }

        private void EyeIcon_Click(object sender, RoutedEventArgs e)
        {
            if (_isPasswordVisible)
            {
                // 切換到隱藏模式
                ApiKeyTextBox.Visibility = Visibility.Collapsed;
                ApiKeyPasswordBox.Visibility = Visibility.Visible;
                ApiKeyPasswordBox.Password = ApiKeyTextBox.Text;

                // 改變圖示為眼睛
                EyeIcon.Source = new BitmapImage(new Uri(@"/Images/SettingsView/EyeOpened.png", UriKind.Relative));
            }
            else
            {
                // 切換到顯示模式
                ApiKeyPasswordBox.Visibility = Visibility.Collapsed;
                ApiKeyTextBox.Visibility = Visibility.Visible;
                ApiKeyTextBox.Text = ApiKeyPasswordBox.Password;

                // 改變圖示為眼睛關閉
                EyeIcon.Source = new BitmapImage(new Uri(@"/Images/SettingsView/EyeClosed.png", UriKind.Relative));
            }

            _isPasswordVisible = !_isPasswordVisible;
        }

        private void ClearIcon_Click(object sender, RoutedEventArgs e)
        {
            ApiKeyPasswordBox.Clear();
            ApiKeyTextBox.Clear();
        }
    }
}

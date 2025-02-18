using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TaiwanWeatherHerald.UserControls
{
    public partial class WeatherMapCard : UserControl
    {
        public WeatherMapCard()
        {
            InitializeComponent();
        }

        public List<ImageSource> Images
        {
            get
            {
                return (List<ImageSource>)GetValue(ImagesProperty);
            }
            set
            {
                SetValue(ImagesProperty, value);
                LoadImageAndUpdateButtonState();
            }
        }
        public static readonly DependencyProperty ImagesProperty = DependencyProperty.Register("Images", typeof(List<ImageSource>), typeof(WeatherMapCard));

        public ImageSource ImageShowed
        {
            get
            {
                return (ImageSource)GetValue(ImageShowedProperty);
            }
            set
            {
                SetValue(ImageShowedProperty, value);
            }
        }
        public static readonly DependencyProperty ImageShowedProperty = DependencyProperty.Register("ImageShowed", typeof(ImageSource), typeof(WeatherMapCard));

        private int _currentIndex = 0; // 當前圖片索引

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                LoadImageAndUpdateButtonState();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex < Images.Count - 1)
            {
                _currentIndex++;
                LoadImageAndUpdateButtonState();
            }
        }

        private void LoadImageAndUpdateButtonState()
        {
            if (Images.Count > 0)
            {
                ImageShowed = Images[_currentIndex];
                PreviousButton.Visibility = _currentIndex > 0 ? Visibility.Visible : Visibility.Hidden;
                NextButton.Visibility = _currentIndex < Images.Count - 1 ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}

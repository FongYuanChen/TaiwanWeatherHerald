using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TaiwanWeatherHerald.Models;

namespace TaiwanWeatherHerald.UserControls
{
    public partial class CCTVMapCard : UserControl
    {
        public CCTVMapCard()
        {
            InitializeComponent();
            InitializeCCTVMap();
            InitializeCCTVTitleMarqueeTimer();
            InitializeCCTVWebViewRefreshTimer();
        }

        public string LocationName
        {
            get
            {
                return (string)GetValue(LocationNameProperty);
            }
            set
            {
                SetValue(LocationNameProperty, value);
                UpdateCCTVMap();
            }
        }
        public static readonly DependencyProperty LocationNameProperty = DependencyProperty.Register("LocationName", typeof(string), typeof(CCTVMapCard));

        public double LocationLongitude
        {
            get
            {
                return (double)GetValue(LocationLongitudeProperty);
            }
            set
            {
                SetValue(LocationLongitudeProperty, value);
                UpdateCCTVMap();
            }
        }
        public static readonly DependencyProperty LocationLongitudeProperty = DependencyProperty.Register("LocationLongitude", typeof(double), typeof(CCTVMapCard));

        public double LocationLatitude
        {
            get
            {
                return (double)GetValue(LocationLatitudeProperty);
            }
            set
            {
                SetValue(LocationLatitudeProperty, value);
                UpdateCCTVMap();
            }
        }
        public static readonly DependencyProperty LocationLatitudeProperty = DependencyProperty.Register("LocationLatitude", typeof(double), typeof(CCTVMapCard));

        public List<CCTVLiveVideoInfo> CCTVInfos
        {
            get
            {
                return (List<CCTVLiveVideoInfo>)GetValue(CCTVInfosProperty);
            }
            set
            {
                SetValue(CCTVInfosProperty, value);
                UpdateCCTVMap();
            }
        }
        public static readonly DependencyProperty CCTVInfosProperty = DependencyProperty.Register("CCTVInfos", typeof(List<CCTVLiveVideoInfo>), typeof(CCTVMapCard));

        #region CCTVMap

        private void InitializeCCTVMap()
        {
            CCTVMap.MapProvider = GMapProviders.OpenStreetMap;
            GMaps.Instance.Mode = AccessMode.ServerOnly;

            CCTVMap.MinZoom = 8;
            CCTVMap.MaxZoom = 16;

            CCTVMap.ShowCenter = false;
            CCTVMap.CanDragMap = true;
            CCTVMap.DragButton = MouseButton.Left;
            CCTVMap.MouseDown += (sender, eventArgs) =>
            {
                if (eventArgs.MiddleButton == MouseButtonState.Pressed)
                {
                    CCTVMap.ZoomAndCenterMarkers(null);
                }
            };
        }

        public void UpdateCCTVMap()
        {
            if (!string.IsNullOrEmpty(LocationName) && LocationLongitude >= 0 && LocationLatitude >= 0 && CCTVInfos != null)
            {
                CCTVMap.Markers.Clear();
                AddLocationMarker(LocationName, LocationLongitude, LocationLatitude);
                foreach (var cctvInfo in CCTVInfos)
                {
                    AddCCTVMarker(cctvInfo);
                }
                CCTVMap.ZoomAndCenterMarkers(null);
            }
        }

        private static readonly int _locationMarkerWidth = 30;
        private static readonly int _locationMarkerHeight = 30;
        private static readonly ImageSource _locationMarkerImageSource = new BitmapImage(new Uri("/Images/CCTVMap/LocationMarker.png", UriKind.Relative));
        private void AddLocationMarker(string name, double longitude, double latitude)
        {
            var markerShape = new Image
            {
                Source = _locationMarkerImageSource,
                Width = _locationMarkerWidth,
                Height = _locationMarkerHeight,
                Cursor = Cursors.Hand,
                ToolTip = $"目標位置: {name}"
            };
            var marker = new GMapMarker(new PointLatLng(latitude, longitude))
            {
                Shape = markerShape,
                Offset = new Point(-_locationMarkerWidth / 2, -_locationMarkerHeight), // Set point to middle bottom of bitmap
            };
            CCTVMap.Markers.Add(marker);
        }

        private static readonly int _cctvMarkerWidth = 30;
        private static readonly int _cctvMarkerHeight = 30;
        private static readonly ImageSource _cctvMarkerImageSource = new BitmapImage(new Uri("/Images/CCTVMap/CCTVMarker.png", UriKind.Relative));
        private void AddCCTVMarker(CCTVLiveVideoInfo info)
        {
            var markerShape = new Image
            {
                Source = _cctvMarkerImageSource,
                Width = _cctvMarkerWidth,
                Height = _cctvMarkerHeight,
                Cursor = Cursors.Hand,
                ToolTip = $"即時影像: {info.Name}"
            };
            markerShape.MouseLeftButtonUp += async (s, e) =>
            {
                await OpenCCTVPopup(info);
            };
            var marker = new GMapMarker(new PointLatLng(info.Latitude, info.Longitude))
            {
                Shape = markerShape,
                Offset = new Point(-_cctvMarkerWidth / 2, -_cctvMarkerHeight), // Set point to middle bottom of bitmap
            };
            CCTVMap.Markers.Add(marker);
        }

        #endregion

        #region CCTVPopup

        private async Task OpenCCTVPopup(CCTVLiveVideoInfo info)
        {
            // 顯示CCTV視窗
            CCTVPopup.Visibility = Visibility.Visible;

            // 重新載入CCTV標題跑馬燈
            CCTVTitle.Text = info.Name;
            StartCCTVTitleMarqueeTimer();

            // 重新載入CCTV WebView2
            if (CCTVWebView.CoreWebView2 != null)
            {
                await CCTVWebView.EnsureCoreWebView2Async();
            }
            CCTVWebView.Source = new Uri(info.Url);
            StartCCTVWebViewRefreshTimer();
        }

        private void CloseCCTVPopup()
        {
            CCTVPopup.Visibility = Visibility.Hidden;
            StopCCTVWebViewRefreshTimer();
            StopCCTVTitleMarqueeTimer();
        }

        private void CloseCCTVPopup_Click(object sender, RoutedEventArgs e)
        {
            CloseCCTVPopup();
        }

        private void RefreshCCTVPopup_Click(object sender, RoutedEventArgs e)
        {
            RefreshCCTVWebView();
        }

        #endregion

        #region CCTVTitleMarqueeTimer

        private DispatcherTimer _cctvTitleMarqueeTimer;
        private readonly TimeSpan _cctvTitleMarqueeUpdateInterval = TimeSpan.FromMilliseconds(30);
        private double _cctvTitlePosition;
        private readonly double _cctvTitleSlidePitch = 2;

        private void InitializeCCTVTitleMarqueeTimer()
        {
            if (_cctvTitleMarqueeTimer == null)
            {
                _cctvTitleMarqueeTimer = new DispatcherTimer
                {
                    Interval = _cctvTitleMarqueeUpdateInterval
                };
                _cctvTitleMarqueeTimer.Tick += (sender, e) => UpdateCCTVTitleMarqueeAnimate();
            }
        }

        private void UpdateCCTVTitleMarqueeAnimate()
        {
            // 設定CCTVTitle位置
            _cctvTitlePosition -= _cctvTitleSlidePitch;
            Canvas.SetLeft(CCTVTitle, _cctvTitlePosition);

            // 如果文字完全移出左側，則重新從右邊開始
            if (_cctvTitlePosition + CCTVTitle.ActualWidth < 0)
            {
                _cctvTitlePosition = CCTVTitleMarqueeContainer.ActualWidth;
            }
        }

        private void StartCCTVTitleMarqueeTimer()
        {
            _cctvTitleMarqueeTimer?.Start();
        }

        private void StopCCTVTitleMarqueeTimer()
        {
            _cctvTitleMarqueeTimer?.Stop();

            // 還原CCTVTitle位置，讓下一次文字從最右邊開始
            _cctvTitlePosition = CCTVTitleMarqueeContainer.ActualWidth;
            Canvas.SetLeft(CCTVTitle, _cctvTitlePosition);
        }

        #endregion

        #region CCTVWebViewRefreshTimer

        private DispatcherTimer _cctvWebViewRefreshTimer;
        private readonly TimeSpan _cctvWebViewRefreshInterval = TimeSpan.FromSeconds(5);

        private void InitializeCCTVWebViewRefreshTimer()
        {
            _cctvWebViewRefreshTimer = new DispatcherTimer
            {
                Interval = _cctvWebViewRefreshInterval
            };
            _cctvWebViewRefreshTimer.Tick += (sender, e) => RefreshCCTVWebView();
        }

        private void StartCCTVWebViewRefreshTimer()
        {
            _cctvWebViewRefreshTimer?.Start();
        }

        private void StopCCTVWebViewRefreshTimer()
        {
            _cctvWebViewRefreshTimer?.Stop();
        }

        private void RefreshCCTVWebView()
        {
            try
            {
                if (CCTVWebView.Source != null && CCTVWebView.CoreWebView2 != null)
                {
                    CCTVWebView.Reload();
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Media.Control;

namespace ui
{
    public partial class MainWindow : Window
    {
        // Oprava CS8618: Inicializace timerů
        private DispatcherTimer _clockTimer = new DispatcherTimer();
        private DispatcherTimer _windowCheckTimer = new DispatcherTimer();
        private GlobalSystemMediaTransportControlsSessionManager? _sessionManager;
        private List<WindowInfo> _windowInfoList = new List<WindowInfo>();

        public class WindowInfo
        {
            public string Title { get; set; } = "";
            public IntPtr Hwnd { get; set; }
            public BitmapSource? Icon { get; set; }
        }

        #region Win32 API
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        private const int SW_RESTORE = 9;
        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            VerifyLogoLoaded();
            PositionAtBottom();
            
            // Inicializace hodin
            _clockTimer.Interval = TimeSpan.FromSeconds(1);
            _clockTimer.Tick += (s, e) => UpdateClock();
            _clockTimer.Start();
            UpdateClock();

            // Inicializace kontroly oken
            _windowCheckTimer.Interval = TimeSpan.FromSeconds(2);
            _windowCheckTimer.Tick += (s, e) => UpdateOpenWindows();
            _windowCheckTimer.Start();
            UpdateOpenWindows();

            UpdateMediaInfo();
        }

        private void VerifyLogoLoaded()
        {
            if (MenuLogo.Source == null)
            {
                System.Diagnostics.Debug.WriteLine("Logo failed to load - using fallback");
                MenuButton.Content = "NOS";
            }
        }

        private void MenuLogo_ImageFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Logo ImageFailed: {e.ErrorException?.Message}");
            MenuLogo.Source = null;
            MenuButton.Content = "NOS";
        }

        private void PositionAtBottom()
        {
            this.Left = 0;
            this.Top = SystemParameters.PrimaryScreenHeight - this.Height;
            this.Width = SystemParameters.PrimaryScreenWidth;
        }

        private void UpdateClock()
        {
            var now = DateTime.Now;
            var cz = new System.Globalization.CultureInfo("cs-CZ");
            
            ClockText.Text = now.ToString("HH:mm");
            DateText.Text = now.ToString("dd/MM");
            
            PopupClockText.Text = now.ToString("HH:mm");
            PopupDateText.Text = now.ToString("dddd, d. MMMM yyyy", cz);
            PopupDayOfYearText.Text = $"Den {now.DayOfYear} z {(DateTime.IsLeapYear(now.Year) ? 366 : 365)}";
        }

        private void UpdateOpenWindows()
        {
            _windowInfoList.Clear();
            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    int length = GetWindowTextLength(hWnd);
                    if (length > 0)
                    {
                        StringBuilder sb = new StringBuilder(length + 1);
                        GetWindowText(hWnd, sb, sb.Capacity);
                        string title = sb.ToString();

                        if (!string.IsNullOrWhiteSpace(title) && 
                            !title.Contains("Nether OS") && 
                            !title.Contains("Program Manager"))
                        {
                            _windowInfoList.Add(new WindowInfo { 
                                Title = title, 
                                Hwnd = hWnd, 
                                Icon = GetWindowIcon(hWnd) 
                            });
                        }
                    }
                }
                return true;
            }, IntPtr.Zero);

            Dispatcher.Invoke(() =>
            {
                OpenWindowsList.ItemsSource = _windowInfoList.ToList();
            });
        }

        private BitmapSource? GetWindowIcon(IntPtr hWnd)
        {
            try
            {
                GetWindowThreadProcessId(hWnd, out uint processId);
                var process = System.Diagnostics.Process.GetProcessById((int)processId);
                string? exePath = process.MainModule?.FileName;
                if (string.IsNullOrEmpty(exePath)) return null;

                SHFILEINFO shinfo = new SHFILEINFO();
                IntPtr hSuccess = SHGetFileInfo(exePath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_LARGEICON);
                if (hSuccess == IntPtr.Zero || shinfo.hIcon == IntPtr.Zero) return null;

                var source = Imaging.CreateBitmapSourceFromHIcon(shinfo.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                source.Freeze();
                DestroyIcon(shinfo.hIcon);
                return source;
            }
            catch { return null; }
        }

        private async void UpdateMediaInfo()
        {
            try {
                _sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                if (_sessionManager != null) {
                    _sessionManager.CurrentSessionChanged += (s, e) => UpdateMediaProperties();
                    UpdateMediaProperties();
                }
            } catch { UpdateNowPlaying("Media service unavailable"); }
        }

        private async void UpdateMediaProperties()
        {
            var session = _sessionManager?.GetCurrentSession();
            if (session != null) {
                var props = await session.TryGetMediaPropertiesAsync();
                Dispatcher.Invoke(() => UpdateNowPlaying($"{props.Title} - {props.Artist}"));
            } else {
                Dispatcher.Invoke(() => UpdateNowPlaying("No music playing"));
            }
        }

        public void UpdateNowPlaying(string songName)
        {
            // Oprava CS0103: Hledání prvku uvnitř Popupu
            if (this.FindName("MenuNowPlayingText") is TextBlock menuText)
            {
                menuText.Text = songName;
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e) => MenuPopup.IsOpen = !MenuPopup.IsOpen;

        private void DateTimeBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) => DateTimePopup.IsOpen = !DateTimePopup.IsOpen;

        private void OpenWindowItem_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement el && el.DataContext is WindowInfo info)
            {
                ShowWindow(info.Hwnd, SW_RESTORE);
                SetForegroundWindow(info.Hwnd);
            }
        }
    }
}
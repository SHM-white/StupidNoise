using System.Configuration;
using System.Data;
using System.Windows;
using H.NotifyIcon;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.IO;
using System.Windows.Controls;

namespace StupidNoise
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string? _tempTrayPngPath;
        private TaskbarIcon? _trayIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                _trayIcon = new TaskbarIcon
                {
                    ToolTipText = "StupidNoise",
                };

                // Build context menu
                var menu = new ContextMenu();
                var toggleItem = new MenuItem { Header = "开关" };
                toggleItem.Click += TrayToggle_Click;
                var exitItem = new MenuItem { Header = "退出" };
                exitItem.Click += TrayExit_Click;
                menu.Items.Add(toggleItem);
                menu.Items.Add(new Separator());
                menu.Items.Add(exitItem);
                _trayIcon.ContextMenu = menu;

                // Try to get vector drawing from resources and render to PNG file, then load via file URI
                if (TryCreateTrayPng(out var pngPath))
                {
                    _tempTrayPngPath = pngPath;
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(pngPath);
                    bitmap.EndInit();
                    _trayIcon.IconSource = bitmap; // File URI -> supported by H.NotifyIcon
                }
            }
            catch
            {
                // Ignore tray creation failures; app can still run without tray
            }
        }

        private bool TryCreateTrayPng(out string pngPath)
        {
            pngPath = string.Empty;
            try
            {
                if (FindResource("TrayIconVector") is DrawingImage vector)
                {
                    const int width = 16;
                    const int height = 16;

                    var dv = new DrawingVisual();
                    using (var dc = dv.RenderOpen())
                    {
                        var bounds = vector.Drawing.Bounds;
                        var scale = Math.Min(width / bounds.Width, height / bounds.Height);
                        var translateX = (width - bounds.Width * scale) / 2 - bounds.X * scale;
                        var translateY = (height - bounds.Height * scale) / 2 - bounds.Y * scale;
                        dc.PushTransform(new TranslateTransform(translateX, translateY));
                        dc.PushTransform(new ScaleTransform(scale, scale));
                        dc.DrawDrawing(vector.Drawing);
                        dc.Pop();
                        dc.Pop();
                    }

                    var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                    rtb.Render(dv);

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    var path = Path.Combine(Path.GetTempPath(), "StupidNoise_tray.png");
                    using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        encoder.Save(fs);
                    }
                    pngPath = path;
                    return true;
                }
            }
            catch
            {
                // ignore
            }
            return false;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                _trayIcon?.Dispose();
            }
            catch { }

            if (!string.IsNullOrEmpty(_tempTrayPngPath))
            {
                try { File.Delete(_tempTrayPngPath); } catch { }
            }

            base.OnExit(e);
        }

        // Handle tray menu clicks from XAML
        private void TrayToggle_Click(object sender, RoutedEventArgs e)
        {
            if (Current?.MainWindow is MainWindow mw)
            {
                mw.IsOnCheckBox.IsChecked = !(mw.IsOnCheckBox.IsChecked ?? false);
            }
        }

        private void TrayExit_Click(object sender, RoutedEventArgs e)
        {
            Current?.Shutdown();
        }
    }

}

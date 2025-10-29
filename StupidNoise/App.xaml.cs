using System.Configuration;
using System.Data;
using System.Windows;
using H.NotifyIcon;

namespace StupidNoise
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Ensure the tray icon resource is instantiated
            _ = FindResource("AppTrayIcon");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Dispose tray icon explicitly
            if (Resources["AppTrayIcon"] is TaskbarIcon icon)
            {
                icon.Dispose();
            }
            base.OnExit(e);
        }

        // Handle tray menu clicks from XAML
        private void TrayToggle_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the main window checkbox if available
            if (Current?.MainWindow is MainWindow mw)
            {
                mw.IsOnCheckBox.IsChecked = !(mw.IsOnCheckBox.IsChecked ?? false);
            }
        }

        private void TrayExit_Click(object sender, RoutedEventArgs e)
        {
            // Gracefully shutdown the application
            Current?.Shutdown();
        }
    }

}

using System.Diagnostics;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace StupidNoise
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Ensure tray resource is instantiated so the icon appears
            Debug.WriteLine(TryFindResource("AppTray").ToString());
            if (TryFindResource("AppTray") is TaskbarIcon trayIcon)
            {
                trayIcon.TrayLeftMouseDown += AppTray_TrayLeftMouseDown;
            }
        }

        private void AppTray_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                MainWindow.Show();
                MainWindow.Activate();
            }
        }

        private void ExitMenu_Click(object sender, RoutedEventArgs e)
        {
            if (TryFindResource("AppViewModel") is AppViewModel vm)
            {
                vm.Dispose();
            }
            Shutdown();
        }
    }
}

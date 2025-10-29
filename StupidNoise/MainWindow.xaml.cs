using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace StupidNoise
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(System.EventArgs e)
        {
            base.OnSourceInitialized(e);
            // Hide the window on startup if desired; comment out if you want it visible
            // this.Hide();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "选择音频文件",
                Filter = "Audio files|*.mp3;*.wav;*.flac;*.wma;*.aac;*.ogg|All files|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                AudioPathTextBox.Text = dlg.FileName;
            }
        }
    }
}
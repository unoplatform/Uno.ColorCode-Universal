using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace ColorCode.WinUITests
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var window = new Window
            {
                Title = "ColorCode.WinUITests",
                Content = new MainPage(),
            };
            window.AppWindow.Resize(new SizeInt32(1400, 1000));
            window.Activate();
        }
    }
}

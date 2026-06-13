using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TechStoreApp.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void RestartButtonClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
        }
    }
}

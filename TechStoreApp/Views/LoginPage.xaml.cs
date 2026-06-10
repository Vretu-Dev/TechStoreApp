using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TechStoreApp.ViewModels;

namespace TechStoreApp.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
            ViewModel.LoginSuccess += ViewModel_LoginSuccess;
        }

        private void ViewModel_LoginSuccess()
        {
            // Update MainWindow UI
            if (App.Current is App app && Window.Current == null)
            {
                // In WinUI 3, Window.Current is null. We need to access the MainWindow instance.
                // However, for simplicity, we can use a static event or just navigate back.
            }
            
            // Navigate back or to Catalog
            Frame.Navigate(typeof(CatalogPage));
            
            // Trigger UI update in MainWindow
            var mainWindow = (Application.Current as App)?.GetMainWindow() as MainWindow;
            mainWindow?.UpdateAuthUI();
        }
    }
}

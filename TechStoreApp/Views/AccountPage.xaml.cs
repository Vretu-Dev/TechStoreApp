using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TechStoreApp.Views
{
    public sealed partial class AccountPage : Page
    {
        public AccountPage()
        {
            this.InitializeComponent();
            ViewModel.LogoutRequested += ViewModel_LogoutRequested;
        }

        private void ViewModel_LogoutRequested()
        {
            var mainWindow = (Application.Current as App)?.GetMainWindow() as MainWindow;
            mainWindow?.UpdateAuthUI();
            Frame.Navigate(typeof(CatalogPage));
        }
    }
}

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TechStoreApp.Views
{
    public sealed partial class AccountPage : Page
    {
        public AccountPage()
        {
            this.InitializeComponent();
            ViewModel.LogoutRequested += () =>
            {
                if (App.Current is App app && app.GetMainWindow() is MainWindow mainWindow)
                {
                    mainWindow.UpdateAuthUI();
                    Frame.Navigate(typeof(LoginPage));
                }
            };
        }
    }
}

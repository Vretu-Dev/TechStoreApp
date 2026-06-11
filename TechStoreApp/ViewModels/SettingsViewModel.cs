using Microsoft.UI.Xaml;
using TechStoreApp.Services;

namespace TechStoreApp.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private int _selectedThemeIndex = (int)SettingsService.Theme;
        public int SelectedThemeIndex
        {
            get => _selectedThemeIndex;
            set
            {
                if (SetProperty(ref _selectedThemeIndex, value))
                {
                    SettingsService.Theme = (ElementTheme)value;
                }
            }
        }

        public SettingsViewModel()
        {
        }
    }
}

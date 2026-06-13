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

        private int _selectedDatabaseModeIndex = SettingsService.IsLocalDatabase ? 0 : 1;
        public int SelectedDatabaseModeIndex
        {
            get => _selectedDatabaseModeIndex;
            set
            {
                if (SetProperty(ref _selectedDatabaseModeIndex, value))
                {
                    SettingsService.IsLocalDatabase = value == 0;
                    OnPropertyChanged(nameof(ExternalConnectionVisibility));
                    IsRestartRequired = true;
                }
            }
        }

        public Visibility ExternalConnectionVisibility => SelectedDatabaseModeIndex == 1 ? Visibility.Visible : Visibility.Collapsed;

        private string _externalConnectionString = SettingsService.ExternalConnectionString;
        public string ExternalConnectionString
        {
            get => _externalConnectionString;
            set
            {
                if (SetProperty(ref _externalConnectionString, value))
                {
                    SettingsService.ExternalConnectionString = value;
                    IsRestartRequired = true;
                }
            }
        }

        private bool _isRestartRequired;
        public bool IsRestartRequired
        {
            get => _isRestartRequired;
            set => SetProperty(ref _isRestartRequired, value);
        }

        public SettingsViewModel()
        {
        }
    }
}

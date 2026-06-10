using System;
using System.Windows.Input;
using TechStoreApp.Services;

namespace TechStoreApp.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        public string FullName => $"{AuthService.CurrentUser?.FirstName} {AuthService.CurrentUser?.LastName}";
        public string Email => AuthService.CurrentUser?.Email ?? string.Empty;

        private string _oldPassword = string.Empty;
        public string OldPassword { get => _oldPassword; set => SetProperty(ref _oldPassword, value); }

        private string _newPassword = string.Empty;
        public string NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }

        private string _message = string.Empty;
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        public ICommand ChangePasswordCommand { get; }
        public ICommand LogoutCommand { get; }

        public event Action? LogoutRequested;

        public AccountViewModel()
        {
            ChangePasswordCommand = new RelayCommand(_ => DoChangePassword());
            LogoutCommand = new RelayCommand(_ => DoLogout());
        }

        private void DoChangePassword()
        {
            if (string.IsNullOrWhiteSpace(OldPassword) || string.IsNullOrWhiteSpace(NewPassword))
            {
                Message = "Wypełnij oba pola hasła.";
                return;
            }

            if (AuthService.ChangePassword(OldPassword, NewPassword))
            {
                Message = "Hasło zostało zmienione.";
                OldPassword = string.Empty;
                NewPassword = string.Empty;
            }
            else
            {
                Message = "Nieprawidłowe stare hasło.";
            }
        }

        private void DoLogout()
        {
            AuthService.Logout();
            LogoutRequested?.Invoke();
        }
    }
}

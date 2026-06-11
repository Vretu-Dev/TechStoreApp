using System;
using System.Windows.Input;
using TechStoreApp.Models;
using TechStoreApp.Services;

namespace TechStoreApp.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        private string _firstName = AuthService.CurrentUser?.FirstName ?? string.Empty;
        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }

        private string _lastName = AuthService.CurrentUser?.LastName ?? string.Empty;
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }

        public string Email => AuthService.CurrentUser?.Email ?? string.Empty;

        private string _oldPassword = string.Empty;
        public string OldPassword { get => _oldPassword; set => SetProperty(ref _oldPassword, value); }

        private string _newPassword = string.Empty;
        public string NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }

        private string _message = string.Empty;
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        public ICommand UpdateInfoCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand LogoutCommand { get; }

        public event Action? LogoutRequested;

        public AccountViewModel()
        {
            UpdateInfoCommand = new RelayCommand(_ => DoUpdateInfo());
            ChangePasswordCommand = new RelayCommand(_ => DoChangePassword());
            LogoutCommand = new RelayCommand(_ => DoLogout());
        }

        private void DoUpdateInfo()
        {
            if (AuthService.CurrentUser == null) return;

            try
            {
                using var db = new TechStoreDbContext();
                var user = db.Customers.Find(AuthService.CurrentUser.CustomerId);
                if (user != null)
                {
                    user.FirstName = FirstName;
                    user.LastName = LastName;
                    db.SaveChanges();
                    
                    // Sync with AuthService
                    AuthService.CurrentUser.FirstName = FirstName;
                    AuthService.CurrentUser.LastName = LastName;
                    
                    Message = "Dane zostały zaktualizowane.";
                }
            }
            catch (Exception ex)
            {
                Message = "Błąd: " + ex.Message;
            }
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

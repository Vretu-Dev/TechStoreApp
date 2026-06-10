using System;
using System.Windows.Input;
using TechStoreApp.Services;

namespace TechStoreApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _isLoginMode = true;
        public bool IsLoginMode
        {
            get => _isLoginMode;
            set
            {
                if (SetProperty(ref _isLoginMode, value))
                {
                    OnPropertyChanged(nameof(IsRegisterMode));
                }
            }
        }

        public bool IsRegisterMode => !IsLoginMode;

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ToggleModeCommand { get; }

        public event Action? LoginSuccess;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(_ => DoLogin());
            RegisterCommand = new RelayCommand(_ => DoRegister());
            ToggleModeCommand = new RelayCommand(_ => IsLoginMode = !IsLoginMode);
        }

        private void DoLogin()
        {
            if (AuthService.Login(Email, Password))
            {
                ErrorMessage = string.Empty;
                LoginSuccess?.Invoke();
            }
            else
            {
                ErrorMessage = "Nieprawidłowy e-mail lub hasło.";
            }
        }

        private void DoRegister()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || 
                string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                ErrorMessage = "Wszystkie pola są wymagane.";
                return;
            }

            if (AuthService.Register(Email, Password, FirstName, LastName))
            {
                ErrorMessage = "Rejestracja udana. Możesz się zalogować.";
                IsLoginMode = true;
            }
            else
            {
                ErrorMessage = "Użytkownik o takim e-mailu już istnieje.";
            }
        }
    }
}

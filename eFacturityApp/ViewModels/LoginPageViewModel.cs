using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
using Prism.Navigation;
using Prism.Services;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;

namespace eFacturityApp.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        [Reactive] public string Email { get; set; }
        [Reactive] public string Password { get; set; }
        [Reactive] public string Version { get; set; }
        public ICommand LoginCommand { get; set; }

        public ICommand RegisterCommand { get; set; }

        public ICommand RecoverAccountCommand { get; set; }

        public LoginPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader)
        {
            Version = $"Versión: {VersionTracking.CurrentVersion} ({VersionTracking.CurrentBuild})";
        }
    }
}

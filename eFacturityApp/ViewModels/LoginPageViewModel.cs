using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
using Prism.Navigation;
using Prism.Services;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using eFacturityApp.Utils;
using System.Threading.Tasks;
using eFacturityApp.Popups.ViewModels;

namespace eFacturityApp.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        [Reactive] public string Email { get; set; } = "LMAO";
        [Reactive] public string Password { get; set; } = "123456";
        [Reactive] public string Version { get; set; }
        public ICommand LoginCommand { get; set; }

        public ICommand RegisterCommand { get; set; }

        public ICommand RecoverAccountCommand { get; set; }

        public LoginPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader)
        {
            Version = $"Versión: {VersionTracking.CurrentVersion} ({VersionTracking.CurrentBuild})";


            RegisterCommand = new Command(async()=> await Utility.Navigate(_navigationService, "RegisterPage"));

            RecoverAccountCommand = new Command(async()=> await Utility.Navigate(_navigationService, "RecoverAccount"));


            LoginCommand = new Command(async()=> await Login());
        }

        private async Task Login()
        {
            try
            {
                if (await ValidateFieldsForm())
                {
                    await Utility.Navigate(_navigationService, "/MainPage/Nav/HomePage");
                }
            }
            catch (Exception err)
            {
                await Utility.ShowAlert("Error - Login", err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
        }


        private async Task<bool> ValidateFieldsForm()
        {
            bool IsValid = false;
            if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password))
            {
                await Utility.ShowAlert("Campos obligatorios", "Deben completarse los campos, para proceder con el ingreso.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
            else
            {
                IsValid = true;
            }

            return IsValid;
        }
    }
}

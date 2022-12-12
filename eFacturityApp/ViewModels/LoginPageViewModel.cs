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
using static eFacturityApp.Infraestructure.ApiModels.APIModels;

namespace eFacturityApp.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        [Reactive] public string User { get; set; }
        [Reactive] public string Password { get; set; }
        [Reactive] public string Version { get; set; }
        public ICommand LoginCommand { get; set; }

        public ICommand RegisterCommand { get; set; }

        public ICommand RecoverAccountCommand { get; set; }

        public LoginPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {
            Version = $"Versión: {VersionTracking.CurrentVersion} ({VersionTracking.CurrentBuild})";

#if DEBUG
            User = "1311263675001";
            Password = "1311263675001";
#endif
            RegisterCommand = new Command(async()=> await Utility.Navigate(_navigationService, "RegisterPage"));

            RecoverAccountCommand = new Command(async()=> await Utility.Navigate(_navigationService, "RecoverAccount"));


            LoginCommand = new Command(async()=> await Login());
        }

        private async Task Login()
        {
            GenericResponseObject<PerfilUsuarioModel> response = new GenericResponseObject<PerfilUsuarioModel>();
            try
            {
                if (await ValidateFieldsForm())
                {
                    await _loaderService.Show("Iniciando sesión..");
                    var TokenResponse = await _apiService.GetToken(User.Trim(), Password.Trim());

                    if (TokenResponse != null)
                    {
                        await _userService.StoreToken(User, TokenResponse.AccessToken, TokenResponse.Expires);
                        response = await _apiService.GetProfileInformation(new LoginUsuarioRequest(User.Trim(), Password.Trim()));
                        await _loaderService.Hide();


                        if (await Utility.HandleAPIResponse(response.statusCode, response.message, "Login", _navigationService))
                        {
                            //Login its OK - Save Token and UserData
                            //response.data.Token = TokenResponse.AccessToken;
                            await _userService.SaveUserInformationProfile(response.data);
                            await Utility.Navigate(_navigationService, "/MainPage/Nav/HomePage"); //correcta pero cae primera vez que hace login
                        }
                        else
                        {
                            await _loaderService.Hide();
                            _userService.CerrarSession();
                        }

                    }
                    else
                    {
                        await Utility.ShowAlert("Error - Login", "Credenciales incorrectas, intente nuevamente." , AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                        _userService.CerrarSession();
                    }
                }
            }
            catch (Exception err)
            {
                await Utility.ShowAlert("Error - Login", "Ocurrio un error inesperado: " +err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
        }


        private async Task<bool> ValidateFieldsForm()
        {
            bool IsValid = false;
            if (string.IsNullOrEmpty(User) && string.IsNullOrEmpty(Password))
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

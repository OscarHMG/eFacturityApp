using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Popups.ViewModels;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using eFacturityApp.Utils;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;

namespace eFacturityApp.ViewModels
{
    public class ChangePasswordPageViewModel : ViewModelBase
    {
        [Reactive] public string Email { get; set; }

        [Reactive] public string NewPassword { get; set; }

        [Reactive] public string ConfirmPassword { get; set; }

        public ICommand ChangePasswordCommand { get; set; }

        public ChangePasswordPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {
            ChangePasswordCommand = new Command(async () => await ChangePasswordAsync());
        }


        public async Task ChangePasswordAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ConfirmPassword) && string.IsNullOrEmpty(NewPassword))
                {
                    await Utility.ShowAlert("Campos obligatorios", "Deben completarse los campos, para proceder con el cambio de clave.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
                else if (ConfirmPassword != NewPassword)
                {
                    await Utility.ShowAlert("Contraseña", "Las contraseñas deben coincidir", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
                else
                {
                    _loaderService.setNavigationService(_navigationService);
                    await _loaderService.Show("Cambiando contraseña..");
                    var UserInformation = await _userService.GetUserInformationProfile();
                    var response = await _apiService.ChangePassword(new ChangePasswordAPIModel(UserInformation.Correo, NewPassword.Trim()));
                    await _loaderService.Hide();
                    if (await Utility.HandleAPIResponse(response.statusCode, response.message, "Cambio de contraseña.", _navigationService))
                    {
                        await Utility.ShowAlert("Cambio contraseña", "Su cambio de contraseña fue exitoso. Por favor intente logear nuevamente, con sus nuevas credenciales", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                        _userService.CerrarSession();
                        await Utility.Navigate(_navigationService, "/Nav/LoginPage");
                    }
                }
            }
            catch (Exception err)
            {
                await _loaderService.Hide();
                await Utility.ShowAlert("Cambio contraseña", err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }

        }
    }
}

using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using eFacturityApp.Utils;
using eFacturityApp.Popups.ViewModels;
using System.Text.RegularExpressions;
using static eFacturityApp.Utils.Utility;
namespace eFacturityApp.ViewModels
{
    public class RecoveryAccountPageViewModel : ViewModelBase
    {
        [Reactive] public string Email { get; set; }
        public ICommand RecoverAccountCommand { get; set; }
        public RecoveryAccountPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {
            RecoverAccountCommand = new Command(async () => await RecoverAccount());
        }


        private async Task RecoverAccount()
        {
            bool Validation = await ValidateFormFields();
            if (Validation)
            {
                try
                {
                    _loaderService.setNavigationService(_navigationService);

                    await _loaderService.Show("Enviando Correo..");
                    var response = await _apiService.RecoverAccount(Email.Trim());
                    await _loaderService.Hide();
                    if (await HandleAPIResponse(response.statusCode, response.message, "Recuperar cuenta", _navigationService))
                    {
                        await Utility.ShowAlert("Recuperar Cuenta", "Se ha generado una contraseña temporal, que se ha sido enviado a su correo. Una vez ingresado, proceda a cambiar su contraseña.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                        await NavigateBack(_navigationService);
                    }


                }
                catch (Exception error)
                {
                    await _loaderService.Hide();
                    await Utility.ShowAlert("Error - Recuperar Cuenta", error.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
        }


        private async Task<bool> ValidateFormFields()
        {
            bool Validation = false;
            if (string.IsNullOrEmpty(Email))
            {
                await Utility.ShowAlert("Campo obligatorio", "El campo email, es necesario para recuperar su cuenta.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
            else if (!Regex.IsMatch(Email.Trim(), @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                await Utility.ShowAlert("Email inválido", "El email ingresado, no es válido.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
            else
            {
                Validation = true;
            }

            return Validation;
        }



    }
}

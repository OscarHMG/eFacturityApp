using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Popups.ViewModels;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;
namespace eFacturityApp.ViewModels
{
    public class NewClientProviderPageViewModel : ViewModelBase
    {
        [Reactive] public DropDown DropDownTipoPersona { get; set; }

        [Reactive] public ItemPicker TipoPersonaSelected { get; set; }
        [Reactive] public PersonaModel PersonaProveedorSelected  { get; set; } = new PersonaModel();

        [Reactive] public string TitlePage { get; set; }
        public ICommand CreateNewClientProviderCommand { get; set; }
        public ICommand LoadDropDownsCommand { get; set; }

        public static string REGEX_EMAIL_PATTERN = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
        public NewClientProviderPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {

            LoadDropDownsCommand = new Command(() => LoadDropDowns());
            CreateNewClientProviderCommand = new Command(async () => await CreateNewClientProvider());

        }




        private void LoadDropDowns()
        {

            List<ItemPicker> TipoPersona = new List<ItemPicker>()
            {
                new ItemPicker(1, "NATURAL", "NATURAL"),
                new ItemPicker(3, "JURÍDICA", "JURÍDICA")
            };

            DropDownTipoPersona = new DropDown(TipoPersona);

            if (PersonaProveedorSelected.IdPersona != 0)
            {
                TipoPersonaSelected = SetSelectedValuesDropDown(PersonaProveedorSelected.IdTipoTipoPersona, TipoPersona);

                this.RaisePropertyChanged("TipoPersonaSelected");
            }
        }

        private async Task<bool> ValidateFields()
        {
            bool isValid = true;

            if (TipoPersonaSelected == null || string.IsNullOrEmpty(PersonaProveedorSelected.Ruc) || string.IsNullOrEmpty(PersonaProveedorSelected.RazonSocial)
                || string.IsNullOrEmpty(PersonaProveedorSelected.NombreComercial) || string.IsNullOrEmpty(PersonaProveedorSelected.Direccion) 
                || string.IsNullOrEmpty(PersonaProveedorSelected.Telefono) || string.IsNullOrEmpty(PersonaProveedorSelected.Correo)) 
            {
                isValid = false;
                await ShowAlert(TitlePage, "Complete los campos, para proceder con el registro.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
            else if (PersonaProveedorSelected.Ruc.Trim().Length != 10 && PersonaProveedorSelected.Ruc.Trim().Length != 13)
            {
                isValid = false;
                await ShowAlert(TitlePage, "RUC, debe constar de 13 dígitos\n Cédula, debe constar con 10 dígitos", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
            else if (!Regex.IsMatch(PersonaProveedorSelected.Correo.Trim(), REGEX_EMAIL_PATTERN, RegexOptions.IgnoreCase))
            {
                isValid = false;
                await ShowAlert(TitlePage, "El formato del correo 1 ingresado, no es válido.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
            else if (!string.IsNullOrEmpty(PersonaProveedorSelected.Correo2))
            {
                if (!Regex.IsMatch(PersonaProveedorSelected.Correo2.Trim(), REGEX_EMAIL_PATTERN, RegexOptions.IgnoreCase))
                {
                    isValid = false;
                    await ShowAlert(TitlePage, "El formato del correo 2 ingresado, no es válido.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                }
            }


            return isValid;
        }


        private async Task CreateNewClientProvider()
        {
            if (await ValidateFields())
            {

                try
                {
                    PersonaProveedorSelected.IdTipoTipoPersona = TipoPersonaSelected.Id;
                    await _loaderService.Show("Un momento..");
                    var response = await _apiService.CreateEditClienteProveedor(PersonaProveedorSelected);

                    if (await HandleAPIResponse(response.statusCode, response.message, TitlePage, _navigationService))
                    {
                        string Message = PersonaProveedorSelected.IdPersona == 0 ? "Su cliente/proveedor fue creado." : "Su cliente/proveedor fue editado correctamente.";
                        await ShowAlert(TitlePage, Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                        NavigationParameters parameters = new NavigationParameters();
                        parameters.Add("Refresh", true);
                        await NavigateBack(_navigationService, parameters);
                    }
                }
                catch (Exception err)
                {
                    await ShowAlert("Error - " + TitlePage, "Error inesperado: " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                }
            }
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            PersonaProveedorSelected = parameters.GetValue<PersonaModel>("PersonaSelected") == null ? new PersonaModel() : parameters.GetValue<PersonaModel>("PersonaSelected");
            TitlePage = PersonaProveedorSelected.IdPersona == 0 ? "Nuevo cliente/proveedor" : "Editar cliente/proveedor";
            LoadDropDownsCommand.Execute(null);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}

using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Popups.ViewModels;
using eFacturityApp.Services;
using eFacturityApp.Utils;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;
namespace eFacturityApp.ViewModels
{
    public class ClientProvidersPageViewModel : ViewModelBase
    {
        [Reactive] public PersonaModel PersonaProveedor { get; set; } = new PersonaModel();
        [Reactive] public ObservableCollection<PersonaModel> ClientProvidersList { get; set; } = new ObservableCollection<PersonaModel>();


        public ICommand LoadClientProviderCommand { get; set; }

        public ICommand SelectedClientProviderCommand { get; set; }

        public ICommand NewClientProviderCommand { get; set; }

        public ICommand DeleteClientProviderCommand { get; set; }

        public ClientProvidersPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {
            LoadClientProviderCommand = new Command(async()=> await LoadClientProvider());

            SelectedClientProviderCommand = new Command<PersonaModel>(async(PersonaSelected)=> 
            {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("PersonaSelected", PersonaSelected);
                await Navigate(_navigationService, "NewClientProviderPage", parameters);
            });

            NewClientProviderCommand = new Command(async()=> await Navigate(_navigationService, "NewClientProviderPage"));

            DeleteClientProviderCommand = new Command<PersonaModel>(async(PersonaSelected) => await DeleteClientProvider(PersonaSelected));
        
        }

        private async Task DeleteClientProvider(PersonaModel personaSelected)
        {
            try
            {
                bool responsePrompt = await ShowYesNoAlert("Eliminar Cliente/proveedor", "¿Está seguro que desea eliminar el cliente escogido?");
                if (responsePrompt)
                {
                    await _loaderService.Show("Un momento..");
                    var response = await _apiService.DeleteProduct(personaSelected.IdPersona);

                    if (await HandleAPIResponse(response.statusCode, response.message, "Cliente/proveedor", _navigationService))
                    {
                        LoadClientProviderCommand.Execute(null);
                    }
                }
            }
            catch (Exception err)
            {
                await ShowAlert("Error - Clientes/Proveedores", "Ocurrió un error inesperado: " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
        }

        private async Task LoadClientProvider()
        {
            try
            {
                await _loaderService.Show("Consultando clientes/proveedores..");
                var response = await _apiService.LoadClienteProveedores();

                if (await Utility.HandleAPIResponse(response.statusCode, response.message, "Clientes/Proveedores", _navigationService))
                {
                    ClientProvidersList = new ObservableCollection<PersonaModel>(response.data);
                }

                await _loaderService.Hide();
            }
            catch (Exception err)
            {
                await Utility.ShowAlert("Error - Clientes/Proveedores", "Ocurrió un error inesperado: " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            LoadClientProviderCommand.Execute(null);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var Refresh = parameters.GetValue<bool>("Refresh");
            if (Refresh)
            {
                LoadClientProviderCommand.Execute(null);
            }
        }
    }
}

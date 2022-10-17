using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Popups.ViewModels;
using eFacturityApp.Services;
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
    public class ConsultaFacturaPageViewModel : ViewModelBase
    {
        [Reactive] public ObservableCollection<FacturaModel> Facturas { get; set; } = new ObservableCollection<FacturaModel>();

        [Reactive] public FiltersApiModel Filtros { get; set; } = new FiltersApiModel();

        public List<ItemPicker> ItemsPersonas { get; set; }

        public List<ItemPicker> Estados { get; set; }
        public ICommand LoadFacturasCommand { get; set; }
        public ICommand ShowFilterPopUpCommand { get; set; }

        [Reactive] public ICommand LoadFiltersCommand { get; set; }


        public ConsultaFacturaPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {
            LoadFacturasCommand = new Command(async () => await LoadFacturas());
            LoadFiltersCommand = new Command(async () => await LoadFilters());
            ShowFilterPopUpCommand = new Command(async () =>
            {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Personas", ItemsPersonas);
                parameters.Add("Estados", Estados);
                await Navigate(_navigationService, "AlertDocumentFiltersPopupPage", parameters);
            });
        }

        private async Task LoadFacturas()
        {
            try
            {
                await _loaderService.Show("Un momento..");
                var response = await _apiService.GetConsultaFacturas(Filtros);
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Consultar facturas", _navigationService))
                {
                    Facturas = new ObservableCollection<FacturaModel>(response.data.Documentos);
                }
                
            }
            catch (Exception err)
            {
                await ShowAlert("Consulta de facturas", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
        }

        private async Task LoadFilters()
        {
            ItemsPersonas = new List<ItemPicker>();
            try
            {
                await _loaderService.Show("Un momento..");
                var response = await _apiService.GetCatalogosFactura();
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Filtros", _navigationService))
                {
                    response.data.Personas.ForEach(x => ItemsPersonas.Add(new ItemPicker(x.IdPersona, x.Ruc.ToUpper(), x.Ruc.ToUpper())));

                    
                }
            }
            catch (Exception err)
            {
                await ShowAlert("Consulta de facturas", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                Estados = new List<ItemPicker>()
                {
                    new ItemPicker(1, "TODAS", "TODAS"),
                    new ItemPicker(2, "PENDIENTES", "PENDIENTES"),
                    new ItemPicker(3, "EMITIDAS", "EMITIDAS"),
                    new ItemPicker(4, "CADUCADAS", "CADUCADAS")
                };
            }
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            LoadFiltersCommand.Execute(null);
            LoadFacturasCommand.Execute(null);

        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            //Validate si viene.
            var currentFilters = parameters.GetValue<FiltersApiModel>("Filtros");
            if (currentFilters != null)
            {
                if (!currentFilters.Estado.Equals(Filtros.Estado) || currentFilters.IdPersona != Filtros.IdPersona ||
                    !currentFilters.Codigo.Equals(Filtros.Codigo))
                {
                    Filtros.Codigo = currentFilters.Codigo;
                    Filtros.Estado = currentFilters.Estado;
                    Filtros.IdPersona = currentFilters.IdPersona;
                    Filtros.IdTipoDocumento = 2;
                    LoadFacturasCommand.Execute(null);
                }
            }
        }
    }
}

using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;
using System.Windows.Input;
using Xamarin.Forms;
using eFacturityApp.Popups.ViewModels;
using System.Threading.Tasks;

namespace eFacturityApp.ViewModels
{
    public class ConsultaProformaPageViewModel : ViewModelBase
    {
        [Reactive] public ObservableCollection<ProformaModel> Proformas { get; set; } = new ObservableCollection<ProformaModel>();

        [Reactive] public FiltersApiModel Filtros { get; set; } = new FiltersApiModel();

        public List<ItemPicker> ItemsPersonas { get; set; }

        public List<ItemPicker> Estados { get; set; }

        [Reactive] public string NoItemsMessage { get; set; } = "";

        [Reactive] public bool IsDownloadingFile { get; set; }
        [Reactive] public double ProgressValue { get; set; }

        public ICommand LoadProformasCommand { get; set; }
        public ICommand ShowFilterPopUpCommand { get; set; }

        public ICommand LoadFiltersCommand { get; set; }

        public ICommand ViewDetalleCommand { get; set; }

        public ConsultaProformaPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {
            LoadProformasCommand = new Command(async () => await LoadProformas());
            LoadFiltersCommand = new Command(async () => await LoadFilters());
            ShowFilterPopUpCommand = new Command(async () =>
            {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Personas", ItemsPersonas);
                parameters.Add("Estados", Estados);
                parameters.Add("FiltersSelected", Filtros);
                parameters.Add("ShowFiltroEstados", false);
                await Navigate(_navigationService, "AlertDocumentFiltersPopupPage", parameters);
            });

            ViewDetalleCommand = new Command<ProformaModel>(async (ProformaSelected) =>
            {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("ProformaDetalle", ProformaSelected);
                await Navigate(_navigationService, "ProformaPage", parameters);
            });
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            LoadFiltersCommand.Execute(null);
            LoadProformasCommand.Execute(null);

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            //Validate si viene.
            var currentFilters = parameters.GetValue<FiltersApiModel>("Filtros");
            if (currentFilters != null)
            {
                if (currentFilters.IdPersona != Filtros.IdPersona || !(currentFilters.Codigo == (Filtros.Codigo)))
                {
                    Filtros.Codigo = currentFilters.Codigo;
                    Filtros.TipoSeleccion = currentFilters.TipoSeleccion;
                    Filtros.IdPersona = currentFilters.IdPersona;
                    Filtros.IdTipoDocumento = 2;
                    LoadProformasCommand.Execute(null);
                }
            }
        }


        private async Task LoadFilters()
        {
            ItemsPersonas = new List<ItemPicker>();
            try
            {
                //await _loaderService.Show("Un momento..");
                var response = await _apiService.GetCatalogosFactura();
                //await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Filtros", _navigationService))
                {
                    response.data.Personas.ForEach(x => ItemsPersonas.Add(new ItemPicker(x.IdPersona, x.Ruc.ToUpper(), x.Ruc.ToUpper())));


                }
            }
            catch (Exception err)
            {
                await ShowAlert("Consulta de proformas", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
        }

        private async Task LoadProformas()
        {
            try
            {
                await _loaderService.Show("Un momento..");
                var response = await _apiService.GetConsultaProformas(Filtros);
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Consultar proformas", _navigationService))
                {
                    Proformas = new ObservableCollection<ProformaModel>(response.data.Cotizaciones);
                    NoItemsMessage = Proformas.Count == 0 ? "No se encontraron proformas con los criterios de búsqueda escogidos." : "";
                }

            }
            catch (Exception err)
            {
                await ShowAlert("Consulta de proformas", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                NoItemsMessage = "Ocurrió un error en la consulta.";
            }
        }

    }
}

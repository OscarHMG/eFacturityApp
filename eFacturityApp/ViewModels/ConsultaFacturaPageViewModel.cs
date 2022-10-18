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

        [Reactive] public string NoItemsMessage { get; set; } = "";
        public ICommand LoadFacturasCommand { get; set; }
        public ICommand ShowFilterPopUpCommand { get; set; }

        public ICommand LoadFiltersCommand { get; set; }

        public ICommand ViewDetalleCommand { get; set; }

        public ICommand DownloadPDFCommand { get; set; }

        public ICommand DownloadXMLCommand { get; set; }

        public ICommand CobrarFacturaCommand { get; set; }

        public ICommand EnviarFacturaSRICommand { get; set; }

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

            ViewDetalleCommand = new Command<FacturaModel>(async (FacturaSelected) =>
            {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("FacturaDetalle", FacturaSelected);
                await Navigate(_navigationService, "FacturaPage", parameters);
            });

            CobrarFacturaCommand = new Command<FacturaModel>(async (FacturaSelected) =>
            {
                var Response = await ShowYesNoAlert("Cobrar factura", "¿Desea enviar a cobro la factura"+ FacturaSelected.Secuencial+  "?", _navigationService);
                if (Response)
                {
                    await CobrarFactura(FacturaSelected);
                }
                
            });

            EnviarFacturaSRICommand = new Command<FacturaModel>(async(FacturaSelected) => 
            {
                var Response = await ShowYesNoAlert("Enviar al SRI", "¿Desea enviar al SRI la factura" + FacturaSelected.Secuencial + "?", _navigationService);
                if (Response)
                {
                    await EnviarSRI(FacturaSelected);
                }
            });

            DownloadPDFCommand = new Command<FacturaModel>(async(FacturaSelected)=> 
            {
                var Response = await ShowYesNoAlert("Factura - Descargar PDF", "¿Desea descargar la factura" + FacturaSelected.Secuencial + "?", _navigationService);
                if (Response)
                {
                    
                }
            });

            DownloadXMLCommand = new Command<FacturaModel>(async (FacturaSelected) =>
            {
                var Response = await ShowYesNoAlert("Factura - Descargar XML", "¿Desea descargar la factura" + FacturaSelected.Secuencial + "?", _navigationService);
                if (Response)
                {

                }
            });
        }

        private async Task EnviarSRI(FacturaModel facturaSelected)
        {
            try
            {
                await _loaderService.Show("Enviando al SRI..");
                var response = await _apiService.EnviarSRIFactura(facturaSelected.IdDocumentoCabecera.GetValueOrDefault());
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Enviar SRI", _navigationService))
                {
                    await ShowAlert("Consulta de facturas", "La  Factura " + facturaSelected.Secuencial + " fue enviada con éxito al SRI.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
            catch (Exception err)
            {
                await ShowAlert("Consulta de facturas", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                LoadFacturasCommand.Execute(null);
            }
        }

        private async Task CobrarFactura(FacturaModel facturaSelected)
        {
            try
            {
                await _loaderService.Show("Cobrando su factura..");
                var response = await _apiService.CobrarFactura(facturaSelected.IdDocumentoCabecera.GetValueOrDefault());
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Cobrar factura", _navigationService))
                {
                    await ShowAlert("Consulta de facturas", "La  Factura " +facturaSelected.Secuencial + " fue cobrada con éxito.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
            catch (Exception err)
            {
                await ShowAlert("Consulta de facturas", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                LoadFacturasCommand.Execute(null);
            }
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
                    NoItemsMessage = Facturas.Count == 0 ? "No se encontraron facturas con los criterios de búsqueda escogidos." : "";
                }

            }
            catch (Exception err)
            {
                await ShowAlert("Consulta de facturas", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                NoItemsMessage = "Ocurrió un error en la consulta.";
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

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
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;

namespace eFacturityApp.ViewModels
{
    public class ConsultaLiquidacionCompraPageViewModel : ViewModelBase
    {
        [Reactive] public ObservableCollection<LiquidacionCompraModel> LiquidacionesCompra { get; set; } = new ObservableCollection<LiquidacionCompraModel>();

        [Reactive] public FiltersApiModel Filtros { get; set; } = new FiltersApiModel();

        public List<ItemPicker> ItemsPersonas { get; set; }

        public List<ItemPicker> Estados { get; set; }

        [Reactive] public string NoItemsMessage { get; set; } = "";

        [Reactive] public bool IsDownloadingFile { get; set; }
        [Reactive] public double ProgressValue { get; set; }

        public ICommand LoadLiquidacionesCompraCommand { get; set; }
        public ICommand ShowFilterPopUpCommand { get; set; }

        public ICommand LoadFiltersCommand { get; set; }

        public ICommand ViewDetalleCommand { get; set; }

        public ICommand DownloadPDFCommand { get; set; }

        public ICommand DownloadXMLCommand { get; set; }

        public ICommand CobrarLiquidacionCompraCommand { get; set; }

        public ICommand EnviarLiquidacionCompraSRICommand { get; set; }

        public ICommand AnularLiquidacionCompraCommand { get; set; }

        public ICommand SendEmailCommand { get; set; }
        public ConsultaLiquidacionCompraPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService/*, downloadService*/)
        {
            LoadLiquidacionesCompraCommand = new Command(async () => await LoadLiquidacionCompras());
            LoadFiltersCommand = new Command(async () => await LoadFilters());
            ShowFilterPopUpCommand = new Command(async () =>
            {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Personas", ItemsPersonas);
                parameters.Add("Estados", Estados);
                parameters.Add("FiltersSelected", Filtros);
                parameters.Add("ShowFiltroEstados", true);
                await Navigate(_navigationService, "AlertDocumentFiltersPopupPage", parameters);
            });

            ViewDetalleCommand = new Command<LiquidacionCompraModel>(async (LiquidacionCompraSelected) =>
            {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("LiquidacionCompraDetalle", LiquidacionCompraSelected);
                await Navigate(_navigationService, "LiquidacionCompraPage", parameters);
            });

            CobrarLiquidacionCompraCommand = new Command<LiquidacionCompraModel>(async (LiquidacionCompraSelected) =>
            {
                var Response = await ShowYesNoAlert("Cobrar liquidación de compra", "¿Desea enviar a cobro la liquidación de compra" + LiquidacionCompraSelected.Secuencial + "?", _navigationService);
                if (Response)
                {
                    await CobrarliquidacionCompra(LiquidacionCompraSelected);
                }

            });

            EnviarLiquidacionCompraSRICommand = new Command<LiquidacionCompraModel>(async (LiquidacionCompraSelected) =>
            {
                var Response = await ShowYesNoAlert("Enviar al SRI", "¿Desea enviar al SRI la liquidación de compra" + LiquidacionCompraSelected.Secuencial + "?", _navigationService);
                if (Response)
                {
                    await EnviarSRI(LiquidacionCompraSelected);
                }
            });

            DownloadPDFCommand = new Command<LiquidacionCompraModel>(async (LiquidacionCompraSelected) =>
            {
                var Response = await ShowYesNoAlert("Liquidación de compra - Descargar PDF", "¿Desea descargar la factura" + LiquidacionCompraSelected.Secuencial + "?", _navigationService);
                if (Response)
                {

                    await StartDownloadAsync(LiquidacionCompraSelected, "pdf");
                }
            });

            DownloadXMLCommand = new Command<LiquidacionCompraModel>(async (LiquidacionCompraSelected) =>
            {
                var Response = await ShowYesNoAlert("Liquidación de compra - Descargar XML", "¿Desea descargar la factura" + LiquidacionCompraSelected.Secuencial + "?", _navigationService);
                if (Response)
                {
                    await StartDownloadAsync(LiquidacionCompraSelected, "xml");
                }
            });

            AnularLiquidacionCompraCommand = new Command<LiquidacionCompraModel>(async (LiquidacionCompraSelected) =>
            {
                var Response = await ShowYesNoAlert("Anular liquidación de compra", "¿Desea anular la liquidación de compra " + LiquidacionCompraSelected.Secuencial + "?", _navigationService);
                if (Response)
                {
                    await AnularLiquidacionCompra(LiquidacionCompraSelected);
                }
            });

            SendEmailCommand = new Command<LiquidacionCompraModel>(async (LiquidacionCompraSelected) => await SendEmail(LiquidacionCompraSelected));
        }



        private async Task SendEmail(LiquidacionCompraModel LiquidacionCompraSelected)
        {
            try
            {
                await _loaderService.Show("Enviando el documento al correo..");

                var userprofileData = await _userService.GetUserInformationProfile();
                EnviarDocumentoDocumentoModel data = new EnviarDocumentoDocumentoModel();
                data.IdDocumento = (long)LiquidacionCompraSelected.IdDocumentoCabeceraLiquidacion;
                data.Tipo = TiposDocumento.Liquidacion;
                data.Correo = userprofileData.Correo;
                var response = await _apiService.EnviarCorreo(data);
                if (await HandleAPIResponse(response.statusCode, response.message, "Envío de Liq. de compra", _navigationService))
                {
                    await ShowAlert("Envío de Liq. de compra", "La Liq. de compra " + LiquidacionCompraSelected.Secuencial + " fue enviada con éxito a su correo.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
            catch (Exception err)
            {
                await ShowAlert("Envío de Liq. de compra", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
        }
        private async Task EnviarSRI(LiquidacionCompraModel liquidacionCompraSelected)
        {
            try
            {
                await _loaderService.Show("Enviando al SRI..");
                var response = await _apiService.EnviarSRIFactura(liquidacionCompraSelected.IdDocumentoCabeceraLiquidacion.GetValueOrDefault());
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Enviar SRI", _navigationService))
                {
                    await ShowAlert("Consulta de liquidación de compra", "La liquidación de compra " + liquidacionCompraSelected.Secuencial + " fue enviada con éxito al SRI.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
            catch (Exception err)
            {
                await ShowAlert("Consulta de liquidación de compra", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                LoadLiquidacionesCompraCommand.Execute(null);
            }
        }


        private async Task AnularLiquidacionCompra(LiquidacionCompraModel liquidacionCompraSelected)
        {
            try
            {
                await _loaderService.Show("Anulando liquidación de compra..");
                var response = await _apiService.AnularLiquidacionCompra(liquidacionCompraSelected.IdDocumentoCabeceraLiquidacion.GetValueOrDefault());
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Anular liquidación de compra", _navigationService))
                {
                    await ShowAlert("Anular liquidación de compra", "La liquidación de compra " + liquidacionCompraSelected.Secuencial + " fue anulada con éxito.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
            catch (Exception err)
            {
                await ShowAlert("Anular liquidación de compra", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                LoadLiquidacionesCompraCommand.Execute(null);
            }
        }

        private async Task CobrarliquidacionCompra(LiquidacionCompraModel liquidacionCompraSelected)
        {
            try
            {
                await _loaderService.Show("Cobrando su liquidación de compra..");
                var response = await _apiService.CobrarLiquidacionCompra(liquidacionCompraSelected.IdDocumentoCabeceraLiquidacion.GetValueOrDefault());
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Cobrar liquidación de compra", _navigationService))
                {
                    await ShowAlert("Consulta de liquidación de compra", "La liquidación de compra " + liquidacionCompraSelected.Secuencial + " fue cobrada con éxito.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
            catch (Exception err)
            {
                await ShowAlert("Consulta de liquidaciones de compra", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                LoadLiquidacionesCompraCommand.Execute(null);
            }
        }

        private async Task LoadLiquidacionCompras()
        {
            try
            {
                await _loaderService.Show("Un momento..");
                var response = await _apiService.GetConsultaLiquidacionCompra(Filtros);
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Consultar liquidaciones de compra", _navigationService))
                {
                    LiquidacionesCompra = new ObservableCollection<LiquidacionCompraModel>(response.data.Documentos);
                    NoItemsMessage = LiquidacionesCompra.Count == 0 ? "No se encontraron liquidaciones de compra con los criterios de búsqueda escogidos." : "";
                }

            }
            catch (Exception err)
            {
                await ShowAlert("Consulta de liquidaciones de compra", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                NoItemsMessage = "Ocurrió un error en la consulta.";
            }
        }

        public async Task StartDownloadAsync(LiquidacionCompraModel liquidacionCompraModel, string extension)
        {
            var progressIndicator = new Progress<double>(ReportProgress);
            var cts = new CancellationTokenSource();
            try
            {
                IsDownloadingFile = true;
                await _loaderService.Show("Descargando documento..");

                //var url = "http://agrega.juntadeandalucia.es/repositorio/01022011/19/es-an_2011020113_9122046/ODE-a52ae1e2-1203-388d-bcc7-51d33d8ffdc4/Biografia_Darwin.pdf";
                var url = string.Format(_apiService.DOWNLOAD_DOC, liquidacionCompraModel.IdDocumentoCabeceraLiquidacion, extension);

                DownloadService downloadService = new DownloadService();
                await downloadService.DownloadFileAsync(url, progressIndicator, cts.Token, liquidacionCompraModel.Secuencial, extension);

                await ShowAlert("Documento descargado", "Su documento ha sido descargado en la carpeta de Descargas", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            catch (OperationCanceledException ex)
            {
                await ShowAlert("Documento descargado", ex.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                //Manage cancellation here
            }
            catch (Exception err)
            {
                await ShowAlert("Documento descargado", err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                IsDownloadingFile = false;
                await _loaderService.Hide();
            }
        }

        internal void ReportProgress(double value)
        {
            ProgressValue = value;
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
                await ShowAlert("Consulta de liquidaciones de compra", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                Estados = new List<ItemPicker>()
                {
                    new ItemPicker(1, "Todos", "Todos"),
                    new ItemPicker(2, "Documentos Emitidos", "Documentos Emitidos"),
                    new ItemPicker(3, "Documentos Pendientes", "Documentos Pendientes"),
                    new ItemPicker(4, "Documentos Anulados", "Documentos Anulados"),
                    new ItemPicker(5, "Documentos Cobrados", "Documentos Cobrados")
                };
            }
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            LoadFiltersCommand.Execute(null);
            LoadLiquidacionesCompraCommand.Execute(null);

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
                if (!(currentFilters.TipoSeleccion == (Filtros.TipoSeleccion)) || currentFilters.IdPersona != Filtros.IdPersona ||
                    !(currentFilters.Codigo == (Filtros.Codigo)))
                {
                    Filtros.Codigo = currentFilters.Codigo;
                    Filtros.TipoSeleccion = currentFilters.TipoSeleccion;
                    Filtros.IdPersona = currentFilters.IdPersona;
                    Filtros.IdTipoDocumento = 2;
                    LoadLiquidacionesCompraCommand.Execute(null);
                }
            }
        }
    }
}

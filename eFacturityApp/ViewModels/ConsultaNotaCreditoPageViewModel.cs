using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using Xamarin.Forms;
using ReactiveUI.Fody.Helpers;
using static eFacturityApp.Utils.Utility;
using System.Collections.ObjectModel;
using System.Windows.Input;
using eFacturityApp.Popups.ViewModels;
using System.Threading.Tasks;
using System.Threading;

namespace eFacturityApp.ViewModels
{
    public class ConsultaNotaCreditoPageViewModel : ViewModelBase
    {
        [Reactive] public ObservableCollection<NotaCreditoModel> NotaCreditos { get; set; } = new ObservableCollection<NotaCreditoModel>();

        [Reactive] public FiltersApiModel Filtros { get; set; } = new FiltersApiModel();

        public List<ItemPicker> ItemsPersonas { get; set; }

        public List<ItemPicker> Estados { get; set; }

        [Reactive] public string NoItemsMessage { get; set; } = "";

        [Reactive] public bool IsDownloadingFile { get; set; }
        [Reactive] public double ProgressValue { get; set; }

        public ICommand LoadNotasCreditosCommand { get; set; }
        public ICommand ShowFilterPopUpCommand { get; set; }

        public ICommand LoadFiltersCommand { get; set; }

        public ICommand ViewDetalleCommand { get; set; }

        public ICommand DownloadPDFCommand { get; set; }

        public ICommand DownloadXMLCommand { get; set; }

        public ICommand CobrarNotaCreditoCommand { get; set; }

        public ICommand EnviarNotaCreditoCommand { get; set; }

        public ICommand AnularNotaCreditoCommand { get; set; }

        public ICommand SendEmailCommand { get; set; }
        public ConsultaNotaCreditoPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService/*, downloadService*/)
        {
            LoadNotasCreditosCommand = new Command(async () => await LoadNotaCreditos());
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

            ViewDetalleCommand = new Command<NotaCreditoModel>(async (NotaCreditoSelected) =>
            {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("NotaCreditoDetalle", NotaCreditoSelected);
                await Navigate(_navigationService, "NotaCreditoPage", parameters);
            });

            CobrarNotaCreditoCommand = new Command<NotaCreditoModel>(async (NotaCreditoSelected) =>
            {
                var Response = await ShowYesNoAlert("Cobrar nota crédito", "¿Desea enviar a cobro la nota crédito" + NotaCreditoSelected.Secuencial + "?", _navigationService);
                if (Response)
                {
                    await CobrarFactura(NotaCreditoSelected);
                }

            });

            EnviarNotaCreditoCommand = new Command<NotaCreditoModel>(async (NotaCreditoSelected) =>
            {
                var Response = await ShowYesNoAlert("Enviar al SRI", "¿Desea enviar al SRI la nota crédito" + NotaCreditoSelected.Secuencial + "?", _navigationService);
                if (Response)
                {
                    await EnviarSRI(NotaCreditoSelected);
                }
            });

            DownloadPDFCommand = new Command<NotaCreditoModel>(async (NotaCreditoSelected) =>
            {
                var Response = await ShowYesNoAlert("Nota crédito - Descargar PDF", "¿Desea descargar la nota crédito" + NotaCreditoSelected.Secuencial + "?", _navigationService);
                if (Response)
                {

                    await StartDownloadAsync(NotaCreditoSelected, "pdf");
                }
            });

            DownloadXMLCommand = new Command<NotaCreditoModel>(async (NotaCreditoSelected) =>
            {
                var Response = await ShowYesNoAlert("Nota crédito - Descargar XML", "¿Desea descargar la nota crédito" + NotaCreditoSelected.Secuencial + "?", _navigationService);
                if (Response)
                {
                    await StartDownloadAsync(NotaCreditoSelected, "xml");
                }
            });

            AnularNotaCreditoCommand = new Command<NotaCreditoModel>(async (NotaCreditoSelected) =>
            {
                var Response = await ShowYesNoAlert("Anular nota crédito", "¿Desea anular la nota crédito " + NotaCreditoSelected.Secuencial + "?", _navigationService);
                if (Response)
                {
                    await AnularFactura(NotaCreditoSelected);
                }
            });

            SendEmailCommand = new Command<NotaCreditoModel>(async (NotaCreditoSelected) => await SendEmail(NotaCreditoSelected));
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
                    new ItemPicker(1, "Todos", "Todos"),
                    new ItemPicker(2, "Documentos Emitidos", "Documentos Emitidos"),
                    new ItemPicker(3, "Documentos Pendientes", "Documentos Pendientes"),
                    new ItemPicker(4, "Documentos Anulados", "Documentos Anulados"),
                    new ItemPicker(5, "Documentos Cobrados", "Documentos Cobrados")
                };
            }
        }



        private async Task SendEmail(NotaCreditoModel NotaCreditoSelected)
        {
            try
            {
                await _loaderService.Show("Enviando el documento al correo..");

                var userprofileData = await _userService.GetUserInformationProfile();
                EnviarDocumentoDocumentoModel data = new EnviarDocumentoDocumentoModel();
                data.IdDocumento = (long)NotaCreditoSelected.IdNotaCreditoCabecera;
                data.Tipo = TiposDocumento.NotaCredito;
                data.Correo = userprofileData.Correo;
                var response = await _apiService.EnviarCorreo(data);
                if (await HandleAPIResponse(response.statusCode, response.message, "Envío de nota crédito", _navigationService))
                {
                    await ShowAlert("Envío de nota crédito", "La nota crédito " + NotaCreditoSelected.Secuencial + " fue enviada con éxito a su correo.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
            catch (Exception err)
            {
                await ShowAlert("Envío de nota crédito", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
        }

        private async Task EnviarSRI(NotaCreditoModel NotaCreditoSelected)
        {
            try
            {
                await _loaderService.Show("Enviando al SRI..");
                var response = await _apiService.EnviarSRIFactura(NotaCreditoSelected.IdNotaCreditoCabecera.GetValueOrDefault());
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Enviar SRI", _navigationService))
                {
                    await ShowAlert("Consulta de nota crédito", "La  nota crédito " + NotaCreditoSelected.Secuencial + " fue enviada con éxito al SRI.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
            catch (Exception err)
            {
                await ShowAlert("Consulta de nota crédito", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                LoadNotasCreditosCommand.Execute(null);
            }
        }


        private async Task AnularFactura(NotaCreditoModel NotaCreditoSelected)
        {
            try
            {
                await _loaderService.Show("Anulando nota crédito..");
                var response = await _apiService.AnularFactura(NotaCreditoSelected.IdNotaCreditoCabecera.GetValueOrDefault());
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Enviar SRI", _navigationService))
                {
                    await ShowAlert("Anular nota crédito", "La nota crédito " + NotaCreditoSelected.Secuencial + " fue anulada con éxito.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
            catch (Exception err)
            {
                await ShowAlert("Anular nota crédito", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                LoadNotasCreditosCommand.Execute(null);
            }
        }

        private async Task CobrarFactura(NotaCreditoModel NotaCreditoSelected)
        {
            try
            {
                await _loaderService.Show("Cobrando su nota crédito..");
                var response = await _apiService.CobrarFactura(NotaCreditoSelected.IdNotaCreditoCabecera.GetValueOrDefault());
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Cobrar nota crédito", _navigationService))
                {
                    await ShowAlert("Consulta de nota crédito", "La nota crédito " + NotaCreditoSelected.Secuencial + " fue cobrada con éxito.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
            catch (Exception err)
            {
                await ShowAlert("Consulta de nota crédito", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                LoadNotasCreditosCommand.Execute(null);
            }
        }

        private async Task LoadNotaCreditos()
        {
            try
            {
                await _loaderService.Show("Un momento..");
                var response = await _apiService.GetConsultaNotaCreditos(Filtros);
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Consultar nota crédito", _navigationService))
                {
                    NotaCreditos = new ObservableCollection<NotaCreditoModel>(response.data.Documentos);
                    NoItemsMessage = NotaCreditos.Count == 0 ? "No se encontraron notas de crédito con los criterios de búsqueda escogidos." : "";
                }

            }
            catch (Exception err)
            {
                await ShowAlert("Consulta de notas de crédito", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                NoItemsMessage = "Ocurrió un error en la consulta.";
            }
        }

        public async Task StartDownloadAsync(NotaCreditoModel notaCreditoModel, string extension)
        {
            var progressIndicator = new Progress<double>(ReportProgress);
            var cts = new CancellationTokenSource();
            try
            {
                IsDownloadingFile = true;
                await _loaderService.Show("Descargando documento..");

                //var url = "http://agrega.juntadeandalucia.es/repositorio/01022011/19/es-an_2011020113_9122046/ODE-a52ae1e2-1203-388d-bcc7-51d33d8ffdc4/Biografia_Darwin.pdf";
                var url = string.Format(_apiService.DOWNLOAD_DOC, notaCreditoModel.IdNotaCreditoCabecera, extension);

                DownloadService downloadService = new DownloadService();
                await downloadService.DownloadFileAsync(url, progressIndicator, cts.Token, notaCreditoModel.Secuencial, extension);

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


        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            LoadFiltersCommand.Execute(null);
            LoadNotasCreditosCommand.Execute(null);

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
                    LoadNotasCreditosCommand.Execute(null);
                }
            }
        }
    }
}

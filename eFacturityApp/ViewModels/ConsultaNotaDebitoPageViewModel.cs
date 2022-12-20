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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;

namespace eFacturityApp.ViewModels
{
    public class ConsultaNotaDebitoPageViewModel : ViewModelBase
    {
        [Reactive] public ObservableCollection<NotaDebitoModel> NotaDebitos { get; set; } = new ObservableCollection<NotaDebitoModel>();

        [Reactive] public FiltersApiModel Filtros { get; set; } = new FiltersApiModel();

        public List<ItemPicker> ItemsPersonas { get; set; }

        public List<ItemPicker> Estados { get; set; }

        [Reactive] public string NoItemsMessage { get; set; } = "";

        [Reactive] public bool IsDownloadingFile { get; set; }
        [Reactive] public double ProgressValue { get; set; }
        public ICommand LoadNotaDebitosCommand { get; set; }
        public ICommand ShowFilterPopUpCommand { get; set; }

        public ICommand LoadFiltersCommand { get; set; }

        public ICommand ViewDetalleCommand { get; set; }

        public ICommand DownloadPDFCommand { get; set; }

        public ICommand DownloadXMLCommand { get; set; }

        public ICommand CobrarNotaDebitoCommand { get; set; }

        public ICommand EnviarFacturaSRICommand { get; set; }

        public ICommand AnularFacturaCommand { get; set; }

        public ICommand SendEmailCommand { get; set; }
        public ConsultaNotaDebitoPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {

            LoadNotaDebitosCommand = new Command(async () => await LoadNotaDebitos());
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

            ViewDetalleCommand = new Command<NotaDebitoModel>(async (NotaDebitoSelected) =>
            {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("NotaDebitoDetalle", NotaDebitoSelected);
                await Navigate(_navigationService, "NotaDebitoPage", parameters);
            });

            CobrarNotaDebitoCommand = new Command<NotaDebitoModel>(async (NotaDebitoSelected) =>
            {
                var Response = await ShowYesNoAlert("Cobrar nota de débito", "¿Desea enviar a cobro la nota de débito" + NotaDebitoSelected.Secuencial + "?", _navigationService);
                if (Response)
                {
                    await CobrarNotaDebito(NotaDebitoSelected);
                }

            });

            EnviarFacturaSRICommand = new Command<NotaDebitoModel>(async (NotaDebitoSelected) =>
            {
                var Response = await ShowYesNoAlert("Enviar al SRI", "¿Desea enviar al SRI la nota de débito" + NotaDebitoSelected.Secuencial + "?", _navigationService);
                if (Response)
                {
                    await EnviarSRI(NotaDebitoSelected);
                }
            });

            DownloadPDFCommand = new Command<NotaDebitoModel>(async (NotaDebitoSelected) =>
            {
                var Response = await ShowYesNoAlert("Nota de débito - Descargar PDF", "¿Desea descargar la nota de débito" + NotaDebitoSelected.Secuencial + "?", _navigationService);
                if (Response)
                {

                    await StartDownloadAsync(NotaDebitoSelected, "pdf");
                }
            });

            DownloadXMLCommand = new Command<NotaDebitoModel>(async (NotaDebitoSelected) =>
            {
                var Response = await ShowYesNoAlert("Nota de débito - Descargar XML", "¿Desea descargar la nota de débito" + NotaDebitoSelected.Secuencial + "?", _navigationService);
                if (Response)
                {
                    await StartDownloadAsync(NotaDebitoSelected, "xml");
                }
            });

            AnularFacturaCommand = new Command<NotaDebitoModel>(async (NotaDebitoSelected) =>
            {
                var Response = await ShowYesNoAlert("Anular Nota de débito", "¿Desea anular la nota de débito " + NotaDebitoSelected.Secuencial + "?", _navigationService);
                if (Response)
                {
                    await AnularNotaDebito(NotaDebitoSelected);
                }
            });

            SendEmailCommand = new Command<NotaDebitoModel>(async (NotaDebitoSelected) => await SendEmail(NotaDebitoSelected));
        }

        internal void ReportProgress(double value)
        {
            ProgressValue = value;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            LoadFiltersCommand.Execute(null);
            LoadNotaDebitosCommand.Execute(null);

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
                    Filtros.IdTipoDocumento = 5;
                    LoadNotaDebitosCommand.Execute(null);
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
                await ShowAlert("Consulta de notas de débito", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

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

        private async Task LoadNotaDebitos()
        {
            try
            {
                await _loaderService.Show("Un momento..");
                var response = await _apiService.GetConsultaNotasDebito(Filtros);
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Consultar notas de débito", _navigationService))
                {
                    NotaDebitos = new ObservableCollection<NotaDebitoModel>(response.data.Documentos);
                    NoItemsMessage = NotaDebitos.Count == 0 ? "No se encontraron notas de débito con los criterios de búsqueda escogidos." : "";
                }

            }
            catch (Exception err)
            {
                await ShowAlert("Consulta de notas de débito", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                NoItemsMessage = "Ocurrió un error en la consulta.";
            }
        }

        private async Task AnularNotaDebito(NotaDebitoModel notaDebitoSelected)
        {
            try
            {
                await _loaderService.Show("Anulando nota de débito..");
                var response = await _apiService.AnularNotaDebito(notaDebitoSelected.IdNotaDebitoCabecera);
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Anular nota de débito", _navigationService))
                {
                    await ShowAlert("Anular nota de débito", "La nota de débito " + notaDebitoSelected.Secuencial + " fue anulada con éxito.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
            catch (Exception err)
            {
                await ShowAlert("Anular nota de débito", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                LoadNotaDebitosCommand.Execute(null);
            }
        }

        private async Task CobrarNotaDebito(NotaDebitoModel notaDebitoSelected)
        {
            try
            {
                await _loaderService.Show("Cobrando su nota de débito..");
                var response = await _apiService.CobrarNotaDebito(notaDebitoSelected.IdNotaDebitoCabecera);
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Cobrar nota de débito", _navigationService))
                {
                    await ShowAlert("Cobrar nota de débito", "La nota de débito " + notaDebitoSelected.Secuencial + " fue cobrada con éxito.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
            catch (Exception err)
            {
                await ShowAlert("Cobrar nota de débito", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                LoadNotaDebitosCommand.Execute(null);
            }
        }

        private async Task EnviarSRI(NotaDebitoModel notaDebitoSelected)
        {
            try
            {
                await _loaderService.Show("Enviando al SRI..");
                var response = await _apiService.EnviarNotaDebitoSRI(notaDebitoSelected.IdNotaDebitoCabecera);
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Enviar SRI", _navigationService))
                {
                    await ShowAlert("Consulta de nota de débito", "La nota de débito " + notaDebitoSelected.Secuencial + " fue enviada con éxito al SRI.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
            catch (Exception err)
            {
                await ShowAlert("Consulta de nota de débito", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                LoadNotaDebitosCommand.Execute(null);
            }
        }

        public async Task StartDownloadAsync(NotaDebitoModel notaDebitoModel, string extension)
        {
            var progressIndicator = new Progress<double>(ReportProgress);
            var cts = new CancellationTokenSource();
            try
            {
                IsDownloadingFile = true;
                await _loaderService.Show("Descargando documento..");

                //var url = "http://agrega.juntadeandalucia.es/repositorio/01022011/19/es-an_2011020113_9122046/ODE-a52ae1e2-1203-388d-bcc7-51d33d8ffdc4/Biografia_Darwin.pdf";
                var url = string.Format(_apiService.DOWNLOAD_DOC_NOTA_DEBITO, notaDebitoModel.IdNotaDebitoCabecera, extension);

                DownloadService downloadService = new DownloadService();
                await downloadService.DownloadFileAsync(url, progressIndicator, cts.Token, notaDebitoModel.Secuencial, extension);

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

        private async Task SendEmail(NotaDebitoModel notaDebitoSelected)
        {
            try
            {
                await _loaderService.Show("Enviando el documento al correo..");

                var userprofileData = await _userService.GetUserInformationProfile();
                EnviarDocumentoDocumentoModel data = new EnviarDocumentoDocumentoModel();
                data.IdDocumento = (long)notaDebitoSelected.IdNotaDebitoCabecera;
                data.Tipo = TiposDocumento.NotaDebito;
                data.Correo = userprofileData.Correo;
                var response = await _apiService.EnviarCorreo(data);
                if (await HandleAPIResponse(response.statusCode, response.message, "Envío de nota de débito", _navigationService))
                {
                    await ShowAlert("Envío de nota de débito", "La nota de débito " + notaDebitoSelected.Secuencial + " fue enviada con éxito a su correo.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
            catch (Exception err)
            {
                await ShowAlert("Envío de nota de débito", "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
        }
    }
}

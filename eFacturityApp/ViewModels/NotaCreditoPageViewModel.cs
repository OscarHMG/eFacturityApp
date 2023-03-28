using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;
using System.Windows.Input;
using ReactiveUI;
using Xamarin.Forms;
using eFacturityApp.Popups.ViewModels;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Xamarin.Essentials.Interfaces;
using eFacturityApp.Utils;
using System.Collections.ObjectModel;
using System.Linq;

namespace eFacturityApp.ViewModels
{
    public class NotaCreditoPageViewModel : ViewModelBase
    {
        [Reactive] public string TitlePage { get; set; }

        [Reactive] public NotaCreditoModel NotaCredito { get; set; } = new NotaCreditoModel();
        [Reactive] public bool EnableControls { get; set; } = true;

        [Reactive] public bool ShowControlesNotaDebitoCreada { get; set; } = false;

        [Reactive] public bool ShowPrompt { get; set; } = true;
        [Reactive] public DropDown DropDownEstablecimientos { get; set; }

        [Reactive] public ItemPicker EstablecimientoSelected { get; set; }

        [Reactive] public DropDown DropDownPuntoVentas { get; set; }

        [Reactive] public ItemPicker PuntoVentaSelected { get; set; }

        [Reactive] public DropDown DropDownPersonas { get; set; }

        [Reactive] public ItemPicker PersonaSelected { get; set; }

        [Reactive] public DropDown DropDownTipoDocumentos { get; set; }

        [Reactive] public ItemPicker TipoDocumentoSelected { get; set; }

        [Reactive] public DropDown DropDownDocumentos { get; set; }

        [Reactive] public ItemPicker DocumentoSelected { get; set; }

        public ICommand LoadDropDownsCommand { get; set; }
        public ICommand CreateNewNotaDebitoCommand { get; set; }

        public ICommand FinalizarCommand { get; set; }

        public ICommand EnviarSRICommand { get; set; }

        public ICommand GetDocumentosCommand { get; set; }

        public ICommand GetDocumentoSelectedDetalleCommand { get; set; }
        public ICommand LoadUserInformationCommand { get; set; }

        [Reactive] public ICommand OpenFilterPopUpCommand { get; set; }

        public ICommand CreateNewNotaCreditoCommand { get; set; }

        DateTime? Desde { get; set; } = null;
        DateTime? Hasta { get; set; } = null;

        PerfilUsuarioModel PerfilUsuario = null;

        [Reactive] public decimal ComprobantevTotal { get; set; }
        [Reactive] public decimal ComprobantevSubtotal0 { get; set; }
        [Reactive] public decimal ComprobantevIvatotal { get; set; }
        [Reactive] public decimal ComprobantevSubtotal12 { get; set; }
        [Reactive] public decimal ComprobantevSubtotal { get; set; }
        [Reactive] public decimal TotalDescuento { get; set; }

        [Reactive] public decimal SubtotalItemsICE { get; set; }
        [Reactive] public string DescuentoString { get; set; } = "0";

        [Reactive] public FacturaModel Factura { get; set; } = new FacturaModel();

        [Reactive] public ObservableCollection<ItemDocumentNotaCreditoCabeceraModel> Items { get; set; } = new ObservableCollection<ItemDocumentNotaCreditoCabeceraModel>();

        public long IdNotaCreditoCreada { get; set; } = 0;

        public NotaCreditoPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {
            LoadDropDownsCommand = new Command(async () => await LoadDropDowns());

            CreateNewNotaCreditoCommand = new Command(async () => await CreateNewNotaCredito());

            //FinalizarCommand = new Command(async () => await NavigateBack(_navigationService));

            //EnviarSRICommand = new Command(async () => await EnviarNotaDebitoSRI());

            this.WhenAnyValue(x => x.EstablecimientoSelected)
                .Where(x => x != null)
                .InvokeCommand(new Command(async () => await LoadPuntosVenta()));

            this.WhenAnyValue(x => x.TipoDocumentoSelected)
                .Where(x => x != null)
                .InvokeCommand(new Command(async () =>
                {
                    GetDocumentosCommand.Execute(null);
                }));

            this.WhenAnyValue(x => x.DocumentoSelected)
                .Where(x => x != null)
                .InvokeCommand(new Command(async () =>
                {
                    GetDocumentoSelectedDetalleCommand.Execute(null);
                }));

            LoadUserInformationCommand = new Command(async () =>
            {
                PerfilUsuario = await userService.GetUserInformationProfile();

            });


            OpenFilterPopUpCommand = new Command(async () =>
            {
                await Navigate(navigationService, "AlertDateFilterPopupPage");
            });


            GetDocumentosCommand = new Command(async () =>
            {
                if (TipoDocumentoSelected != null)
                {
                    if (TipoDocumentoSelected.Id == 1)
                    {
                        await GetDocumentosNotaCreditoFacturaAsync();
                    }
                    else if (TipoDocumentoSelected.Id == 2)
                    {
                        await GetDocumentosNotaCreditoLiqCompraAsync();
                    }
                }

            });

            GetDocumentoSelectedDetalleCommand = new Command(async () =>
            {
                if (TipoDocumentoSelected != null)
                {
                    if (TipoDocumentoSelected.Id == 1)
                    {
                        await GetDetalleFacturaSelected();
                    }
                    else if (TipoDocumentoSelected.Id == 2)
                    {
                        await GetDetalleLiqCompraSelected();
                    }
                }
            });
        }


        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            TitlePage = "Nueva nota crédito";


            var NotaCreditoDetalle = parameters.GetValue<NotaCreditoModel>("NotaCreditoDetalle");
            if (NotaCreditoDetalle != null)
            {
                NotaCredito = NotaCreditoDetalle;
                EnableControls = false;
                TitlePage = "Detalle nota crébito";
                ShowPrompt = false;
            }
            LoadDropDownsCommand.Execute(null);
            LoadUserInformationCommand.Execute(null);
        }


        private async Task LoadDropDowns()
        {
            List<ItemPicker> ItemsEstablecimiento = new List<ItemPicker>();
            List<ItemPicker> ItemsPuntoVentas = new List<ItemPicker>();
            List<ItemPicker> ItemsTipoDocumentos = new List<ItemPicker>();
            try
            {
                await _loaderService.Show("Un momento..");
                var response = await _apiService.GetCatalogosFactura();

                if (await HandleAPIResponse(response.statusCode, response.message, "Nota crébito", _navigationService))
                {
                    response.data.Establecimientos.ForEach(x => ItemsEstablecimiento.Add(new ItemPicker(x.IdEstablecimiento, (x.Codigo + " " + x.Nombre).ToUpper(), (x.Codigo + " " + x.Nombre).ToUpper())));
                    response.data.PuntosVenta.ForEach(x => ItemsPuntoVentas.Add(new ItemPicker(x.IdPuntoVenta, (x.Codigo + " " + x.Nombre).ToUpper(), (x.Codigo + " " + x.Nombre).ToUpper())));

                    await _loaderService.Hide();
                }
            }
            catch (Exception err)
            {
                await ShowAlert(TitlePage, "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                DropDownEstablecimientos = new DropDown(ItemsEstablecimiento);
                DropDownPuntoVentas = new DropDown(ItemsPuntoVentas);
                DropDownTipoDocumentos = new DropDown(LoadTipoDocumentos());
                if (NotaCredito.IdNotaCreditoCabecera != 0)
                {
                    EstablecimientoSelected = SetSelectedValuesDropDown(NotaCredito.IdEstablecimiento, ItemsEstablecimiento);
                    PuntoVentaSelected = SetSelectedValuesDropDown(NotaCredito.IdPuntoVenta, ItemsPuntoVentas);

                    this.RaisePropertyChanged("EstablecimientoSelected");
                    this.RaisePropertyChanged("PuntoVentaSelected");
                }
            }
        }

        private List<ItemPicker> LoadTipoDocumentos()
        {
            List<ItemPicker> Items = new List<ItemPicker>();
            ItemPicker D1 = new ItemPicker
            {
                Id = 1,
                TextoCorto = "Factura",
                TextoLargo = "Factura"
            };
            ItemPicker D2 = new ItemPicker
            {
                Id = 2,
                TextoCorto = "Liq. de compra de bienes o servicios",
                TextoLargo = "Liq. de compra de bienes o servicios"
            };

            Items.Add(D1);
            Items.Add(D2);
            return Items;
        }


        private async Task LoadPuntosVenta()
        {
            List<ItemPicker> ItemsPuntoVentas = new List<ItemPicker>();
            try
            {
                DropDownPuntoVentas = new DropDown();
                await _loaderService.Show("Consultando puntos de venta..");
                var response = await _apiService.GetCatalogosPuntoVenta(EstablecimientoSelected.Id);
                await _loaderService.Hide();

                if (await HandleAPIResponse(response.statusCode, response.message, TitlePage, _navigationService))
                {
                    response.data.ForEach(x => ItemsPuntoVentas.Add(new ItemPicker(x.IdPuntoVenta, (x.Codigo + " " + x.Nombre).ToUpper(), (x.Codigo + " " + x.Nombre).ToUpper())));
                }
            }
            catch (Exception err)
            {
                await ShowAlert(TitlePage, "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
            finally
            {
                DropDownPuntoVentas = new DropDown(ItemsPuntoVentas);
            }
        }
        private async Task GetDocumentosNotaCreditoFacturaAsync()
        {
            try
            {

                if ((Desde == null || Desde.Value == DateTime.MinValue) || (Hasta == null || Hasta.Value == DateTime.MinValue))
                {
                    await ShowAlert(TitlePage, "Debe escoger fechas desde-hasta de filtro de documentos. Para eso, presionar en la lupa para definir en que fechas buscar", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                }
                else
                {
                    if (PerfilUsuario == null || PerfilUsuario.IdEmpresa == 0)
                    {
                        await Utility.ShowAlert("Error", "Sus credenciales han expirado. Por favor, ingrese nuevamente sus credenciales.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                        _userService.CerrarSession();
                        await Utility.Navigate(_navigationService, "/Nav/LoginPage");

                        return;
                    }


                    await _loaderService.Show("Consultando facturas..");
                    DropDownDocumentos = new DropDown();
                    var response = await _apiService.GetFacturasNotaCredito(PerfilUsuario.IdEmpresa, Desde.Value, Hasta.Value);
                    await _loaderService.Hide();
                    if (await HandleAPIResponse(response.statusCode, response.message, TitlePage, _navigationService))
                    {
                        if (response.data.Count > 0)
                        {
                            List<ItemPicker> Items = new List<ItemPicker>();
                            response.data.ForEach(x =>
                            {


                                ItemPicker D1 = new ItemPicker
                                {
                                    Id = x.IdDocumentoCabeceraRelacionado,
                                    TextoCorto = x.Secuencial,
                                    TextoLargo = x.Secuencial
                                };
                                Items.Add(D1);
                            });

                            DropDownDocumentos = new DropDown(Items);
                        }
                    }
                }

            }
            catch (Exception err)
            {
                await ShowAlert(TitlePage, "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
        }


        private async Task GetDocumentosNotaCreditoLiqCompraAsync()
        {
            try
            {
                if ((Desde == null || Desde.Value == DateTime.MinValue) || (Hasta == null || Hasta.Value == DateTime.MinValue))
                {
                    await ShowAlert(TitlePage, "Debe escoger fechas desde-hasta de filtro de documentos. Para eso, presionar en la lupa para definir en que fechas buscar", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                }
                else
                {

                    if (PerfilUsuario == null || PerfilUsuario.IdEmpresa == 0)
                    {
                        await Utility.ShowAlert("Error", "Sus credenciales han expirado. Por favor, ingrese nuevamente sus credenciales.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                        _userService.CerrarSession();
                        await Utility.Navigate(_navigationService, "/Nav/LoginPage");

                        return;
                    }

                    await _loaderService.Show("Consultando liquidaciones de compra..");
                    DropDownDocumentos = new DropDown();
                    var response = await _apiService.GetLiqCompraNotaCredito(PerfilUsuario.IdEmpresa, Desde.Value, Hasta.Value);
                    await _loaderService.Hide();
                    if (await HandleAPIResponse(response.statusCode, response.message, TitlePage, _navigationService))
                    {
                        if (response.data.Count > 0)
                        {
                            List<ItemPicker> Items = new List<ItemPicker>();
                            response.data.ForEach(x =>
                            {


                                ItemPicker D1 = new ItemPicker
                                {
                                    Id = x.IdDocumentoCabeceraRelacionado,
                                    TextoCorto = x.Secuencial,
                                    TextoLargo = x.Secuencial
                                };
                                Items.Add(D1);
                            });

                            DropDownDocumentos = new DropDown(Items);
                        }
                    }
                }


            }
            catch (Exception err)
            {
                await ShowAlert(TitlePage, "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
        }



        private async Task GetDetalleFacturaSelected()
        {
            try
            {

                await _loaderService.Show("Consultando factura..");
                DropDownDocumentos = new DropDown();
                var response = await _apiService.GetDetalleNotaCreditoFactura(DocumentoSelected.Id);
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, TitlePage, _navigationService))
                {
                    var DataResponse = response.data;
                    ComprobantevTotal = DataResponse.ComprobantevTotal;
                    TotalDescuento = DataResponse.TotalDescuento;
                    ComprobantevSubtotal0 = DataResponse.ComprobantevSubtotal0;
                    ComprobantevIvatotal = DataResponse.ComprobantevIvatotal;
                    ComprobantevSubtotal12 = DataResponse.ComprobantevSubtotal12;
                    ComprobantevSubtotal = DataResponse.ComprobantevSubtotal;
                    SubtotalItemsICE = 0;
                    Items = new ObservableCollection<ItemDocumentNotaCreditoCabeceraModel>(DataResponse.Items);
                }
            }
            catch (Exception err)
            {
                await ShowAlert(TitlePage, "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
        }


        private async Task GetDetalleLiqCompraSelected()
        {
            try
            {

                await _loaderService.Show("Consultando liq. de compra...");
                DropDownDocumentos = new DropDown();
                var response = await _apiService.GetDetalleNotaCreditoliqCompra(DocumentoSelected.Id);
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, TitlePage, _navigationService))
                {
                    var DataResponse = response.data;
                    ComprobantevTotal = DataResponse.ComprobantevTotal;
                    TotalDescuento = DataResponse.TotalDescuento;
                    ComprobantevSubtotal0 = DataResponse.ComprobantevSubtotal0;
                    ComprobantevIvatotal = DataResponse.ComprobantevIvatotal;
                    ComprobantevSubtotal12 = DataResponse.ComprobantevSubtotal12;
                    ComprobantevSubtotal = DataResponse.ComprobantevSubtotal;
                    SubtotalItemsICE = 0;
                    Items = new ObservableCollection<ItemDocumentNotaCreditoCabeceraModel>(DataResponse.Items);

                    Items = new ObservableCollection<ItemDocumentNotaCreditoCabeceraModel>(DataResponse.Items);
                }
            }
            catch (Exception err)
            {
                await ShowAlert(TitlePage, "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
        }


        private async Task CreateNewNotaCredito()
        {
            try
            {
                bool isValid = ValidateFields().First().Key;
                string Message = ValidateFields().First().Value;
                if (ValidateFields().First().Key)
                {
                    _loaderService.setNavigationService(_navigationService);
                    await _loaderService.Show("Registrando su nota de crédito..");
                    NotaCredito.IdEstablecimiento = EstablecimientoSelected.Id;
                    NotaCredito.IdPuntoVenta = PuntoVentaSelected.Id;
                    NotaCredito.IdDocumentoCabeceraRelacionado = DocumentoSelected.Id;
                    var response = await _apiService.CreateNewNotaCredito(NotaCredito);
                    await _loaderService.Hide();

                    if (await HandleAPIResponse(response.statusCode, response.message, TitlePage, _navigationService))
                    {

                        await ShowAlert(TitlePage, "Su nota de crédito fue creada de manera exitosa.", Popups.ViewModels.AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                        if (response.data.IdNotaCreditoCabecera != 0)
                        {
                            //Bloqueo todos los controles
                            EnableControls = false;
                            TitlePage = "Detalle de nota de Crédito";
                            IdNotaCreditoCreada = response.data.IdNotaCreditoCabecera.GetValueOrDefault();
                            ShowControlesNotaDebitoCreada = true;
                        }
                    }
                }
                else
                {
                    await ShowAlert(TitlePage, Message, Popups.ViewModels.AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }

            }
            catch (Exception e)
            {
                await ShowAlert(TitlePage, e.Message, Popups.ViewModels.AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
        }


        private Dictionary<bool, string> ValidateFields()
        {
            bool isValid = true;
            string ErrorMessage = "";
            Dictionary<bool, string> KeyValueValidation = new Dictionary<bool, string>();
            if (NotaCredito.FechaEmision == null)
            {
                isValid = false;
                ErrorMessage = "Fecha emisión, es requerido.";
            }
            else if (EstablecimientoSelected == null || EstablecimientoSelected.Id == 0)
            {
                isValid = false;
                ErrorMessage = "Establecimiento, es requerido.";
            }
            else if (PuntoVentaSelected == null || PuntoVentaSelected.Id == 0)
            {
                isValid = false;
                ErrorMessage = "Punto de venta, es requerido.";
            }
            else if (DocumentoSelected == null || DocumentoSelected.Id == 0)
            {
                isValid = false;
                ErrorMessage = "Secuencial del documento, es requerido.";
            }
            
            KeyValueValidation.Add(isValid, ErrorMessage);
            return KeyValueValidation;
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            Desde = parameters.GetValue<DateTime>("Desde");
            Hasta = parameters.GetValue<DateTime>("Hasta");
            bool ApplyFilters = parameters.GetValue<bool>("ApplyFilters");
            if (ApplyFilters)
            {
                GetDocumentosCommand.Execute(null);
            }

        }
    }
}

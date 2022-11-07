using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Popups.ViewModels;
using eFacturityApp.Services;
using eFacturityApp.Views;
using Prism.AppModel;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;

namespace eFacturityApp.ViewModels
{
    public class FacturaPageViewModel : ViewModelBase, IPageLifecycleAware
    {
        [Reactive] public string TitlePage { get; set; }

        [Reactive] public FacturaModel Factura { get; set; } = new FacturaModel();
        [Reactive] public DropDown DropDownFormasPago { get; set; }

        [Reactive] public ItemPicker FormasPagoSelected { get; set; }
        [Reactive] public DropDown DropDownEstablecimientos { get; set; }

        [Reactive] public ItemPicker EstablecimientoSelected { get; set; }

        [Reactive] public DropDown DropDownPuntoVentas { get; set; }

        [Reactive] public ItemPicker PuntoVentaSelected { get; set; }

        [Reactive] public DropDown DropDownPersonas { get; set; }

        [Reactive] public ItemPicker PersonaSelected { get; set; }

        [Reactive] public List<ProductoModel> ProductosServicios { get; set; } = new List<ProductoModel>();
        [Reactive] public ObservableCollection<ItemFacturaModel> ItemsFactura { get; set; } = new ObservableCollection<ItemFacturaModel>();
        [Reactive] public bool EnableControls { get; set; } = true;

        [Reactive] public bool ShowPrompt { get; set; } = true;

        [Reactive] public bool ShowControlesFacturaCreada { get; set; } = false;
        public long IdFacturaCreada { get; set; } = 0;

        [Reactive] public bool IsLoadingTotales { get; set; } = false;
        public ICommand LoadDropDownsCommand { get; set; }

        public ICommand NewItemCommand { get; set; }

        public ICommand RemoveItemCommand { get; set; }

        public ICommand CreateNewFacturaCommand { get; set; }

        public ICommand FinalizarCommand { get; set; }

        public ICommand EnviarSRICommand { get; set; }

        public ICommand CalcularTotalesCommand { get; set; }

        public ICommand EntryDescuentoUnfocusedCommand { get; protected set; }


        //Valores de Totales
        [Reactive] public decimal SubtotalItemsMasIva { get; set; }
        [Reactive] public decimal SubtotalItemsIva { get; set; }
        [Reactive] public decimal SubtotalItemsICE { get; set; }
        [Reactive] public decimal SubtotalItemsCeroIva { get; set; }
        [Reactive] public decimal SubtotalItemsNoGrabaIva { get; set; }
        [Reactive] public decimal TotalDocumentoElectronico { get; set; }
        [Reactive] public decimal TotalDescuento { get; set; }

        [Reactive] public string DescuentoString { get; set; } = "0";

        public FacturaPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {
            LoadDropDownsCommand = new Command(async () => await LoadDropDowns());

            NewItemCommand = new Command(async () =>
            {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("DescuentoGlobal", Factura.PorcentajeDescuento);
                parameters.Add("ListProductos", ProductosServicios);
                await Navigate(_navigationService, "AlertFacturaItemPopupPage", parameters);
            });

            RemoveItemCommand = new Command<ItemFacturaModel>(async (Item) => await DeleteItem(Item));

            CreateNewFacturaCommand = new Command(async () => await CreateNewFactura());


            FinalizarCommand = new Command(async () => await NavigateBack(_navigationService));

            EnviarSRICommand = new Command(async () => await EnviarFacturaSRI());

            CalcularTotalesCommand = new Command(async () => await CalcularTotales());

            this.WhenAnyValue(x => x.EstablecimientoSelected)
                .Where(x => x != null)
                .InvokeCommand(new Command(async () => await LoadPuntosVenta()));

            this.WhenAnyValue(x => x.DescuentoString)
                .Where(x => string.IsNullOrEmpty(x))
                .InvokeCommand(new Command(() => DescuentoString = "0"));


            EntryDescuentoUnfocusedCommand = new Command(()=> RecalcularValores());

            


        }


        private void RecalcularValores()
        {
            if (ItemsFactura.Count != 0)
            {
                var List = ItemsFactura.ToList();
                List.ForEach(c => c.Descuento = Factura.PorcentajeDescuento.GetValueOrDefault());
                ItemsFactura = new ObservableCollection<ItemFacturaModel>(List);
                CalcularTotalesCommand.Execute(null);
            }
        }

        private async Task EnviarFacturaSRI()
        {
            try
            {
                await _loaderService.Show("Enviando factura al SRI..");
                var response = await _apiService.EnviarSRIFactura(IdFacturaCreada);
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Envío SRI", _navigationService))
                {
                    await ShowAlert("Envío SRI", "Su factura fue enviada al SRI de manera exitosa.", Popups.ViewModels.AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                    await NavigateBack(_navigationService);
                }
            }
            catch (Exception err)
            {
                await ShowAlert(TitlePage, err.Message, Popups.ViewModels.AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
        }

        private async Task LoadDropDowns()
        {
            List<ItemPicker> ItemsFormasPago = new List<ItemPicker>();
            List<ItemPicker> ItemsEstablecimiento = new List<ItemPicker>();
            List<ItemPicker> ItemsPuntoVentas = new List<ItemPicker>();
            List<ItemPicker> ItemsPersonas = new List<ItemPicker>();
            try
            {
                await _loaderService.Show("Un momento..");
                var response = await _apiService.GetCatalogosFactura();

                if (await HandleAPIResponse(response.statusCode, response.message, "Factura", _navigationService))
                {
                    response.data.FormasPago.ForEach(x => ItemsFormasPago.Add(new ItemPicker(x.IdFormaPago, (x.Codigo + " " + x.Nombre).ToUpper(), (x.Codigo + " " + x.Nombre).ToUpper())));
                    response.data.Establecimientos.ForEach(x => ItemsEstablecimiento.Add(new ItemPicker(x.IdEstablecimiento, (x.Codigo + " " + x.Nombre).ToUpper(), (x.Codigo + " " + x.Nombre).ToUpper())));
                    response.data.Personas.ForEach(x => ItemsPersonas.Add(new ItemPicker(x.IdPersona, x.Ruc.ToUpper(), x.Ruc.ToUpper())));
                    response.data.PuntosVenta.ForEach(x => ItemsPuntoVentas.Add(new ItemPicker(x.IdPuntoVenta, (x.Codigo + " " + x.Nombre).ToUpper(), (x.Codigo + " " + x.Nombre).ToUpper())));


                    ProductosServicios = new List<ProductoModel>(response.data.Productos);
                    await _loaderService.Hide();


                }
            }
            catch (Exception err)
            {
                await ShowAlert(TitlePage, "Error : " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            finally
            {
                DropDownFormasPago = new DropDown(ItemsFormasPago);
                DropDownEstablecimientos = new DropDown(ItemsEstablecimiento);
                DropDownPersonas = new DropDown(ItemsPersonas);
                //Dropdown, dependiente de Establecimiento sea escogido.
                DropDownPuntoVentas = new DropDown(ItemsPuntoVentas);

                if (Factura.IdDocumentoCabecera != 0)
                {
                    FormasPagoSelected = SetSelectedValuesDropDown(Factura.IdFormaPago, ItemsFormasPago);
                    EstablecimientoSelected = SetSelectedValuesDropDown(Factura.IdEstablecimiento, ItemsEstablecimiento);
                    PuntoVentaSelected = SetSelectedValuesDropDown(Factura.IdPuntoVenta, ItemsPuntoVentas);
                    PersonaSelected = SetSelectedValuesDropDown(Factura.IdPersona, ItemsPersonas);
                    this.RaisePropertyChanged("FormasPagoSelected");
                    this.RaisePropertyChanged("EstablecimientoSelected");
                    this.RaisePropertyChanged("PuntoVentaSelected");
                    this.RaisePropertyChanged("PersonaSelected");

                }
            }

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


        private async Task CalcularTotales()
        {
            try
            {
                IsLoadingTotales = true;
                var response = await _apiService.CalcularTotales(ItemsFactura.ToList());
                if (response.statusCode == 200)
                {
                    var Totales = response.data;
                    SubtotalItemsMasIva = Totales.SubtotalItemsMasIva;
                    SubtotalItemsIva = Totales.SubtotalItemsIva;
                    SubtotalItemsICE = Totales.SubtotalItemsICE;
                    SubtotalItemsCeroIva = Totales.SubtotalItemsCeroIva;
                    SubtotalItemsNoGrabaIva = Totales.SubtotalItemsNoGrabaIva;
                    TotalDocumentoElectronico = Totales.TotalDocumentoElectronico;
                    TotalDescuento = Totales.TotalDescuento;
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                IsLoadingTotales = false;
            }
        }


        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            TitlePage = "Nueva factura";


            var FacturaDetalle = parameters.GetValue<FacturaModel>("FacturaDetalle");
            if (FacturaDetalle != null)
            {
                Factura = FacturaDetalle;
                ItemsFactura = new ObservableCollection<ItemFacturaModel>(Factura.Items);
                EnableControls = false;
                TitlePage = "Detalle de Factura";
                ShowPrompt = false;
                //Cargar totales desde el API
                TotalDocumentoElectronico = Factura.ComprobantevTotal;
                SubtotalItemsMasIva = Factura.ComprobantevSubtotal;
                SubtotalItemsIva = Factura.ComprobantevIvatotal;
                SubtotalItemsICE = Factura.ComprobantevICEtotal;
                SubtotalItemsCeroIva = Factura.ComprobantevSubtotal0;
                SubtotalItemsNoGrabaIva = 0; // Revisar con castillo.
                TotalDescuento = Factura.TotalDescuento;


            }
            LoadDropDownsCommand.Execute(null);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            //Llega con un item nuevo?
            ItemFacturaModel NewItem = parameters.GetValue<ItemFacturaModel>("NewItemFactura");
            AddNewItemCreated(NewItem);

        }


        public void AddNewItemCreated(ItemFacturaModel NewItem)
        {
            if (NewItem != null)
            {
                ItemsFactura.Add(NewItem);
                CalcularTotalesCommand.Execute(null);
            }
        }

        private async Task DeleteItem(ItemFacturaModel Item)
        {
            if (Factura.IdDocumentoCabecera != null && Factura.IdDocumentoCabecera != 0)
            {
                await ShowAlert("Detalle de factura", "No se puede modificar la factura, en modo VISUALIZADOR", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
            else
            {
                var Response = await ShowYesNoAlert("Eliminar item", "¿Desea eliminar este item?", _navigationService);
                if (Response)
                {
                    var FilteredList = ItemsFactura.ToList().Where(x => !x.GUID.Equals(Item.GUID));
                    ItemsFactura = new ObservableCollection<ItemFacturaModel>(FilteredList);
                    if (ItemsFactura.Count != 0)
                    {
                        CalcularTotalesCommand.Execute(null);

                    }
                    else
                    {
                        //Sino hay items, encerar todo..
                        TotalDocumentoElectronico = 0;
                        SubtotalItemsMasIva = 0;
                        SubtotalItemsIva = 0;
                        SubtotalItemsICE = 0;
                        SubtotalItemsCeroIva = 0;
                        SubtotalItemsNoGrabaIva = 0;
                        TotalDescuento = 0;
                    }
                }
            }
        }

        private Dictionary<bool, string> ValidateFields()
        {
            bool isValid = true;
            string ErrorMessage = "";
            Dictionary<bool, string> KeyValueValidation = new Dictionary<bool, string>();
            if (Factura.FechaEmision == null)
            {
                isValid = false;
                ErrorMessage = "Fecha emisión, es requerido.";
            }
            else if (Factura.DiasCredito == null)
            {
                isValid = false;
                ErrorMessage = "Días de crédito, es requerido.";
            }
            else if (FormasPagoSelected == null)
            {
                isValid = false;
                ErrorMessage = "Forma de pago, es requerido.";
            }
            else if (EstablecimientoSelected == null)
            {
                isValid = false;
                ErrorMessage = "Establecimiento, es requerido.";
            }
            else if (PuntoVentaSelected == null)
            {
                isValid = false;
                ErrorMessage = "Punto de venta, es requerido.";
            }
            else if (Factura.PorcentajeDescuento == null)
            {
                isValid = false;
                ErrorMessage = "% de descuento, es requerido.";
            }
            else if (PersonaSelected == null)
            {
                isValid = false;
                ErrorMessage = "Persona, es requerido.";
            }
            else if (ItemsFactura.Count == 0)
            {
                isValid = false;
                ErrorMessage = "La factura debe contar con al menos un item en su detalle.";
            }
            KeyValueValidation.Add(isValid, ErrorMessage);
            return KeyValueValidation;
        }

        private async Task CreateNewFactura()
        {
            try
            {
                bool isValid = ValidateFields().First().Key;
                string Message = ValidateFields().First().Value;
                if (ValidateFields().First().Key)
                {
                    _loaderService.setNavigationService(_navigationService);
                    await _loaderService.Show("Registrando su factura..");
                    Factura.Items = ItemsFactura.ToList();
                    Factura.IdFormaPago = FormasPagoSelected.Id;
                    Factura.IdEstablecimiento = EstablecimientoSelected.Id;
                    Factura.IdPuntoVenta = PuntoVentaSelected.Id;
                    Factura.IdPersona = PersonaSelected.Id;
                    var response = await _apiService.CreateNewFactura(Factura);
                    await _loaderService.Hide();

                    if (await HandleAPIResponse(response.statusCode, response.message, TitlePage, _navigationService))
                    {

                        await ShowAlert(TitlePage, "Su factura fue creada de manera exitosa.", Popups.ViewModels.AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                        if (response.data.IdDocumentoCabecera != null)
                        {
                            //Bloqueo todos los controles
                            EnableControls = false;
                            TitlePage = "Detalle de Factura";
                            IdFacturaCreada = response.data.IdDocumentoCabecera.GetValueOrDefault();
                            ShowControlesFacturaCreada = true;
                        }

                    }
                    //await NavigateBack(_navigationService);
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

        public void OnAppearing()
        {
            MessagingCenter.Subscribe<FacturaPage, decimal>(this, "ValorDesc", (sender, arg) =>
            {
                Factura.PorcentajeDescuento = arg;
                this.RaisePropertyChanged("Factura.PorcentajeDescuento");
                RecalcularValores();
            });
        }

        public void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<FacturaPage, decimal>(this, "ValorDesc");
        }
    }



}

using System;
using eFacturityApp.Infraestructure;
using eFacturityApp.Services;
using eFacturityApp.Infraestructure.Services;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;
using eFacturityApp.Popups.ViewModels;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using ReactiveUI;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Prism.AppModel;
using System.Linq;
using eFacturityApp.Views;
using DryIoc;

namespace eFacturityApp.ViewModels
{
    public class ProformaPageViewModel : ViewModelBase, IPageLifecycleAware
    {
        [Reactive] public string TitlePage { get; set; }
        [Reactive] public ProformaModel Proforma { get; set; } = new ProformaModel();
        [Reactive] public DropDown DropDownFormasPago { get; set; }
        [Reactive] public ItemPicker FormasPagoSelected { get; set; }
        [Reactive] public DropDown DropDownEstablecimientos { get; set; }
        [Reactive] public ItemPicker EstablecimientoSelected { get; set; }
        [Reactive] public DropDown DropDownPuntoVentas { get; set; }
        [Reactive] public ItemPicker PuntoVentaSelected { get; set; }
        [Reactive] public DropDown DropDownPersonas { get; set; }
        [Reactive] public ItemPicker PersonaSelected { get; set; }
        [Reactive] public List<ProductoModel> ProductosServicios { get; set; } = new List<ProductoModel>();
        [Reactive] public ObservableCollection<ItemProforma> ItemsProforma { get; set; } = new ObservableCollection<ItemProforma>();
        [Reactive] public bool EnableControls { get; set; } = true;
        [Reactive] public bool ShowPrompt { get; set; } = true;
        [Reactive] public bool ShowControlesFacturaCreada { get; set; } = false;
        public long IdFacturaCreada { get; set; } = 0;
        [Reactive] public bool IsLoadingTotales { get; set; } = false;

        public ICommand LoadDropDownsCommand { get; set; }
        public ICommand NewItemCommand { get; set; }
        public ICommand RemoveItemCommand { get; set; }
        public ICommand CreateNewProformaCommand { get; set; }
        public ICommand CalcularTotalesCommand { get; set; }
        public ICommand EntryDescuentoUnfocusedCommand { get; protected set; }

        public ProformaPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {
            LoadDropDownsCommand = new Xamarin.Forms.Command(async () => await LoadDropDowns());
            NewItemCommand = new Command(async () =>
            {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("DescuentoGlobal", Proforma.PorcentajeDescuento);
                parameters.Add("ListProductos", ProductosServicios);
                parameters.Add("PageToReturn", "ProformaPage");
                await Navigate(_navigationService, "AlertFacturaItemPopupPage", parameters);
            });

            RemoveItemCommand = new Command<ItemProforma>(async (Item) => await DeleteItem(Item));
            CalcularTotalesCommand = new Command(async () => await CalcularTotales());
            EntryDescuentoUnfocusedCommand = new Command(() => RecalcularValores());
        }


        public void OnAppearing()
        {
            MessagingCenter.Subscribe<ProformaPage, decimal>(this, "ValorDesc", (sender, arg) =>
            {
                Proforma.PorcentajeDescuento = arg;
                this.RaisePropertyChanged("Proforma.PorcentajeDescuento");
                RecalcularValores();
            });
        }


        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            TitlePage = "Nueva proforma";


            var ProformaDetalle = parameters.GetValue<ProformaModel>("ProformaDetalle");
            if (ProformaDetalle != null)
            {
                Proforma = ProformaDetalle;
                ItemsProforma  = new ObservableCollection<ItemProforma>(Proforma.Items);
                EnableControls = false;
                TitlePage = "Detalle de Proforma";
                ShowPrompt = false;
                //Cargar totales desde el API
                //TotalDocumentoElectronico = Factura.ComprobantevTotal;
                //SubtotalItemsMasIva = Factura.ComprobantevSubtotal;
                //SubtotalItemsIva = Factura.ComprobantevIvatotal;
                //SubtotalItemsICE = Factura.ComprobantevICEtotal;
                //SubtotalItemsCeroIva = Factura.ComprobantevSubtotal0;
                //SubtotalItemsNoGrabaIva = 0; // Revisar con castillo.
                //TotalDescuento = Factura.TotalDescuento;


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
            ItemProforma NewItem = parameters.GetValue<ItemProforma>("NewItemProforma");
            AddNewItemCreated(NewItem);

        }
        public void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<ProformaPage, decimal>(this, "ValorDesc");
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

                if (await HandleAPIResponse(response.statusCode, response.message, "Proforma", _navigationService))
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

                if (Proforma.IdProformaCabecera != 0)
                {
                    FormasPagoSelected = SetSelectedValuesDropDown(Proforma.IdFormaPago, ItemsFormasPago);
                    EstablecimientoSelected = SetSelectedValuesDropDown(Proforma.IdEstablecimiento, ItemsEstablecimiento);
                    PuntoVentaSelected = SetSelectedValuesDropDown(Proforma.IdPuntoVenta, ItemsPuntoVentas);
                    PersonaSelected = SetSelectedValuesDropDown(Proforma.IdPersona, ItemsPersonas);
                    this.RaisePropertyChanged("FormasPagoSelected");
                    this.RaisePropertyChanged("EstablecimientoSelected");
                    this.RaisePropertyChanged("PuntoVentaSelected");
                    this.RaisePropertyChanged("PersonaSelected");
                }
            }

        }

        private Dictionary<bool, string> ValidateFields()
        {
            bool isValid = true;
            string ErrorMessage = "";
            Dictionary<bool, string> KeyValueValidation = new Dictionary<bool, string>();
            if (Proforma.FechaEmision == null)
            {
                isValid = false;
                ErrorMessage = "Fecha emisión, es requerido.";
            }
            else if (Proforma.DiasCredito == null)
            {
                isValid = false;
                ErrorMessage = "Días de crédito, es requerido.";
            }
            else if (Proforma.PorcentajeDescuento == null)
            {
                isValid = false;
                ErrorMessage = "% de descuento, es requerido.";
            }
            else if (PersonaSelected == null)
            {
                isValid = false;
                ErrorMessage = "Persona, es requerido.";
            }
            else if (ItemsProforma.Count == 0)
            {
                isValid = false;
                ErrorMessage = "La factura debe contar con al menos un item en su detalle.";
            }
            KeyValueValidation.Add(isValid, ErrorMessage);
            return KeyValueValidation;
        }

        public void AddNewItemCreated(ItemProforma NewItem)
        {
            if (NewItem != null)
            {
                ItemsProforma.Add(NewItem);
                CalcularTotalesCommand.Execute(null);
            }
        }
        private async Task DeleteItem(ItemProforma Item)
        {
            if (Proforma.IdProformaCabecera != null && Proforma.IdProformaCabecera != 0)
            {
                await ShowAlert("Detalle de proforma", "No se puede modificar la proforma, en modo VISUALIZADOR", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
            else
            {
                var Response = await ShowYesNoAlert("Eliminar item", "¿Desea eliminar este item?", _navigationService);
                if (Response)
                {
                    var FilteredList = ItemsProforma.ToList().Where(x => !x.GUID.Equals(Item.GUID));
                    if (FilteredList == null)
                    {
                        ItemsProforma = new ObservableCollection<ItemProforma>();
                    }
                    else
                    {
                        ItemsProforma = new ObservableCollection<ItemProforma>(FilteredList);
                    }
                    
                    /*if (ItemsProforma.Count != 0)
                    {
                        CalcularTotalesCommand.Execute(null);

                    }
                    else
                    {
                        //Sino hay items, encerar TODO..
                        TotalDocumentoElectronico = 0;
                        SubtotalItemsMasIva = 0;
                        SubtotalItemsIva = 0;
                        SubtotalItemsICE = 0;
                        SubtotalItemsCeroIva = 0;
                        SubtotalItemsNoGrabaIva = 0;
                        TotalDescuento = 0;
                    }*/
                }
            }
        }

        private void RecalcularValores()
        {
            if (ItemsProforma.Count != 0)
            {
                var List = ItemsProforma.ToList();
                List.ForEach(c => c.Descuento = Proforma.PorcentajeDescuento.GetValueOrDefault());
                ItemsProforma = new ObservableCollection<ItemProforma>(List);
                CalcularTotalesCommand.Execute(null);
            }
        }


        private async Task CalcularTotales()
        {
            /*try
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
            }*/
        }
    }
}


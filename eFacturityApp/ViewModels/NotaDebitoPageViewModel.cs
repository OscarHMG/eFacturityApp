using DryIoc;
using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Popups.ViewModels;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;

namespace eFacturityApp.ViewModels
{
    public class NotaDebitoPageViewModel : ViewModelBase
    {
        [Reactive] public string TitlePage { get; set; }

        [Reactive] public NotaDebitoModel NotaDebito { get; set; } = new NotaDebitoModel();
        [Reactive] public bool EnableControls { get; set; } = true;

        [Reactive] public bool ShowControlesNotaDebitoCreada { get; set; } = false;

        [Reactive] public bool ShowPrompt { get; set; } = true;
        public long IdNotaDebitoCreada { get; set; } = 0;
        [Reactive] public DropDown DropDownEstablecimientos { get; set; }

        [Reactive] public ItemPicker EstablecimientoSelected { get; set; }

        [Reactive] public DropDown DropDownPuntoVentas { get; set; }

        [Reactive] public ItemPicker PuntoVentaSelected { get; set; }

        [Reactive] public DropDown DropDownPersonas { get; set; }

        [Reactive] public ItemPicker PersonaSelected { get; set; }

        [Reactive] public DropDown DropDownImpuestos { get; set; }

        [Reactive] public ItemPicker ImpuestoSelected { get; set; }

        public ICommand LoadDropDownsCommand { get; set; }
        public ICommand CreateNewNotaDebitoCommand { get; set; }

        public ICommand FinalizarCommand { get; set; }

        public ICommand EnviarSRICommand { get; set; }
        public NotaDebitoPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {
            LoadDropDownsCommand = new Command(async () => await LoadDropDowns());

            CreateNewNotaDebitoCommand = new Command(async () => await CreateNewNotaDebito());

            FinalizarCommand = new Command(async () => await NavigateBack(_navigationService));

            EnviarSRICommand = new Command(async () => await EnviarNotaDebitoSRI());

            this.WhenAnyValue(x => x.EstablecimientoSelected)
                .Where(x => x != null)
                .InvokeCommand(new Command(async () => await LoadPuntosVenta()));

            
        }


        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            TitlePage = "Nueva nota débito";


            var NotaDebitoDetalle = parameters.GetValue<NotaDebitoModel>("NotaDebitoDetalle");
            if (NotaDebitoDetalle != null)
            {
                NotaDebito = NotaDebitoDetalle;
                EnableControls = false;
                TitlePage = "Detalle nota débito";
                ShowPrompt = false;
            }
            LoadDropDownsCommand.Execute(null);
        }


        private async Task LoadDropDowns()
        {
            List<ItemPicker> ItemsFormasPago = new List<ItemPicker>();
            List<ItemPicker> ItemsEstablecimiento = new List<ItemPicker>();
            List<ItemPicker> ItemsPuntoVentas = new List<ItemPicker>();
            List<ItemPicker> ItemsPersonas = new List<ItemPicker>();
            List<ItemPicker> ItemsImpuestos = new List<ItemPicker>();
            try
            {
                await _loaderService.Show("Un momento..");
                var response = await _apiService.GetCatalogosFactura();

                if (await HandleAPIResponse(response.statusCode, response.message, "Nota débito", _navigationService))
                {
                    response.data.FormasPago.ForEach(x => ItemsFormasPago.Add(new ItemPicker(x.IdFormaPago, (x.Codigo + " " + x.Nombre).ToUpper(), (x.Codigo + " " + x.Nombre).ToUpper())));
                    response.data.Establecimientos.ForEach(x => ItemsEstablecimiento.Add(new ItemPicker(x.IdEstablecimiento, (x.Codigo + " " + x.Nombre).ToUpper(), (x.Codigo + " " + x.Nombre).ToUpper())));
                    response.data.Personas.ForEach(x => ItemsPersonas.Add(new ItemPicker(x.IdPersona, x.Ruc.ToUpper(), x.Ruc.ToUpper())));
                    response.data.PuntosVenta.ForEach(x => ItemsPuntoVentas.Add(new ItemPicker(x.IdPuntoVenta, (x.Codigo + " " + x.Nombre).ToUpper(), (x.Codigo + " " + x.Nombre).ToUpper())));
                    response.data.Impuestos.ForEach(x => ItemsImpuestos.Add(new ItemPicker(x.IdImpuesto, (x.CodigoImpuesto + "-" + x.NombreImpuesto), (x.CodigoImpuesto + "-" + x.NombreImpuesto))));

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
                DropDownPersonas = new DropDown(ItemsPersonas);
                //Dropdown, dependiente de Establecimiento sea escogido.
                DropDownImpuestos = new DropDown(ItemsImpuestos);
                DropDownPuntoVentas = new DropDown(ItemsPuntoVentas);

                if (NotaDebito.IdNotaDebitoCabecera != 0)
                {
                    EstablecimientoSelected = SetSelectedValuesDropDown(NotaDebito.IdEstablecimiento, ItemsEstablecimiento);
                    PuntoVentaSelected = SetSelectedValuesDropDown(NotaDebito.IdPuntoVenta, ItemsPuntoVentas);
                    PersonaSelected = SetSelectedValuesDropDown(NotaDebito.IdPersona, ItemsPersonas);
                    ImpuestoSelected = SetSelectedValuesDropDown(NotaDebito.IdImpuesto, ItemsImpuestos);
                    
                    this.RaisePropertyChanged("EstablecimientoSelected");
                    this.RaisePropertyChanged("PuntoVentaSelected");
                    this.RaisePropertyChanged("PersonaSelected");
                    this.RaisePropertyChanged("ImpuestoSelected");
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

        private async Task CreateNewNotaDebito()
        {
            try
            {
                bool isValid = ValidateFields().First().Key;
                string Message = ValidateFields().First().Value;
                if (ValidateFields().First().Key)
                {
                    _loaderService.setNavigationService(_navigationService);
                    await _loaderService.Show("Registrando su nota de débito..");
                    NotaDebito.IdEstablecimiento = EstablecimientoSelected.Id;
                    NotaDebito.IdPuntoVenta = PuntoVentaSelected.Id;
                    NotaDebito.IdPersona = PersonaSelected.Id;
                    NotaDebito.IdImpuesto = ImpuestoSelected.Id;
                    var response = await _apiService.CreateNewNotaDebito(NotaDebito);
                    await _loaderService.Hide();

                    if (await HandleAPIResponse(response.statusCode, response.message, TitlePage, _navigationService))
                    {

                        await ShowAlert(TitlePage, "Su nota de débito fue creada de manera exitosa.", Popups.ViewModels.AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                        if (response.data.IdNotaDebitoCabecera != 0)
                        {
                            //Bloqueo todos los controles
                            EnableControls = false;
                            TitlePage = "Detalle de nota de débito";
                            IdNotaDebitoCreada = response.data.IdNotaDebitoCabecera;
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
            if (NotaDebito.FechaEmision == null)
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
            else if (PersonaSelected == null || PersonaSelected.Id == 0)
            {
                isValid = false;
                ErrorMessage = "Persona, es requerido.";
            }
            else if (ImpuestoSelected == null || ImpuestoSelected.Id == 0)
            {
                isValid = false;
                ErrorMessage = "Impuesto, es requerido.";
            }
            else if (NotaDebito.DocumentoModificaValorModificacion == null)
            {
                isValid = false;
                ErrorMessage = "Valor de modificación, es requerido.";
            }
            KeyValueValidation.Add(isValid, ErrorMessage);
            return KeyValueValidation;
        }

        private async Task EnviarNotaDebitoSRI()
        {
            try
            {
                await _loaderService.Show("Enviando nota de débito al SRI..");
                var response = await _apiService.EnviarNotaDebitoSRI(IdNotaDebitoCreada);
                await _loaderService.Hide();
                if (await HandleAPIResponse(response.statusCode, response.message, "Envío SRI", _navigationService))
                {
                    await ShowAlert("Envío nota de débito", "Su nota de débito fue enviada al SRI de manera exitosa.", Popups.ViewModels.AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                    await NavigateBack(_navigationService);
                }
            }
            catch (Exception err)
            {
                await ShowAlert(TitlePage, err.Message, Popups.ViewModels.AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
        }
    }
}

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

namespace eFacturityApp.ViewModels
{
    public class NotaCreditoPageViewModel : ViewModelBase
    {
        [Reactive] public string TitlePage { get; set; }

        [Reactive] public NotaCreditoModel NotaCredito { get; set; } = new NotaCreditoModel();
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

        [Reactive] public DropDown DropDownTipoDocumentos { get; set; }

        [Reactive] public ItemPicker TipoDocumentoSelected { get; set; }

        public ICommand LoadDropDownsCommand { get; set; }
        public ICommand CreateNewNotaDebitoCommand { get; set; }

        public ICommand FinalizarCommand { get; set; }

        public ICommand EnviarSRICommand { get; set; }


        public NotaCreditoPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
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
    }
}

using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Popups.ViewModels;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;

namespace eFacturityApp.ViewModels
{
    public class NewProductPageViewModel : ViewModelBase
    {
        [Reactive] public string TitlePage { get; set; }
        [Reactive] public DropDown DropDownTipoArticulo { get; set; }

        [Reactive] public ItemPicker TipoArticuloSelected { get; set; }

        [Reactive] public DropDown DropDownImpuestos { get; set; }

        [Reactive] public ItemPicker ImpuestoSelected { get; set; }

        [Reactive] public DropDown DropDownUnidadMedidas { get; set; }

        [Reactive] public ItemPicker UnidadMedidaSelected { get; set; }

        public ICommand CreateNewProductoCommand { get; set; }
        public ICommand LoadDropDownsCommand { get; set; }

        [Reactive] public ProductoModel Product { get; set; } = new ProductoModel();


        public NewProductPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {
            LoadDropDownsCommand = new Command(async() => await LoadDropDowns());

            CreateNewProductoCommand = new Command(async () => await CreateNewProduct());


        }

        private async Task CreateNewProduct()
        {
            if (await ValidateFields())
            {
                try
                {

                    Product.IdTipoArticulo = TipoArticuloSelected.Id;
                    Product.IdImpuesto = ImpuestoSelected.Id;
                    Product.IdUnidadMedida = UnidadMedidaSelected.Id;

                    await _loaderService.Show("Un momento..");
                    var response = await _apiService.CreateEditProduct(Product);

                    if (await HandleAPIResponse(response.statusCode, response.message, TitlePage, _navigationService))
                    {
                        string Message = Product.IdProducto == 0 ? "Su producto fue creado." : "Su producto fue editado correctamente.";
                        await ShowAlert(TitlePage, Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                        NavigationParameters parameters = new NavigationParameters();
                        parameters.Add("Refresh", true);
                        await NavigateBack(_navigationService, parameters);
                    }
                }
                catch (Exception err) 
                {
                    await ShowAlert("Error - "+TitlePage, "Error inesperado: " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                }
            }
        }

        private async Task LoadDropDowns()
        {
            List<ItemPicker> TipoArticulos = new List<ItemPicker>();
            List<ItemPicker> Impuestos = new List<ItemPicker>();
            List<ItemPicker> UnidadMedidas = new List<ItemPicker>();

            DropDownTipoArticulo = new DropDown();
            DropDownImpuestos = new DropDown();
            DropDownUnidadMedidas = new DropDown();

            try
            {
                var response = await _apiService.GetCatalogosProducto();

                if (await HandleAPIResponse(response.statusCode, response.message, TitlePage, _navigationService))
                {
                    response.data.TiposArticulos.ForEach(x=> 
                    {
                        TipoArticulos.Add(new ItemPicker(x.IdTipoArticulo, (x.CodigoArticulo + " " + x.Nombre).ToUpper(), (x.CodigoArticulo + " " + x.Nombre).ToUpper()));
                    });

                    response.data.TiposImpuestos.ForEach(x => 
                    {
                        Impuestos.Add(new ItemPicker(x.IdImpuesto, (x.IdImpuesto + " " + x.Nombre).ToUpper(), (x.IdImpuesto + " " + x.Nombre).ToUpper())); 
                    });
                    response.data.UnidadesMedida.ForEach(x => 
                    { 
                        UnidadMedidas.Add(new ItemPicker(x.IdUnidadMedida, x.Nombre.ToUpper(), x.Nombre.ToUpper())); 
                    });

                    DropDownTipoArticulo = new DropDown(TipoArticulos);
                    DropDownImpuestos = new DropDown(Impuestos);
                    DropDownUnidadMedidas = new DropDown(UnidadMedidas);

                    if (Product.IdProducto != 0)
                    {
                        TipoArticuloSelected = SetSelectedValuesDropDown(Product.IdTipoArticulo, TipoArticulos);
                        ImpuestoSelected = SetSelectedValuesDropDown(Product.IdImpuesto, Impuestos);
                        UnidadMedidaSelected = SetSelectedValuesDropDown(Product.IdUnidadMedida, UnidadMedidas);
                        this.RaisePropertyChanged("TipoArticuloSelected");
                        this.RaisePropertyChanged("ImpuestoSelected");
                        this.RaisePropertyChanged("UnidadMedidaSelected");
                    }
                }
                else
                {
                    await ShowAlert(TitlePage, "Ocurrió un problema en la consulta. Inténtelo más tarde.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                    await NavigateBack(_navigationService);
                }
            }
            catch (Exception err)
            {
                await ShowAlert(TitlePage, "Error : "+ err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
            }
        }

        private async Task<bool> ValidateFields()
        {
            bool isValid = true;
            if (TipoArticuloSelected == null || string.IsNullOrEmpty(Product.Nombre) || ImpuestoSelected == null ||
                string.IsNullOrEmpty(Product.Codigo) || /*UnidadMedidaSelected == null ||*/ string.IsNullOrEmpty(Product.CodigoAuxiliar)
                )
            {
                isValid = false;
                await ShowAlert(TitlePage, "Complete los campos, para proceder con el registro.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
            else
            {
                isValid = true;
            }

            return isValid;
        }



        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            Product = parameters.GetValue<ProductoModel>("ProductSelected") == null ? new ProductoModel() : parameters.GetValue<ProductoModel>("ProductSelected");
            TitlePage = Product.IdProducto == 0 ? "Nuevo producto" : "Editar producto";
            LoadDropDownsCommand.Execute(null);
        }


        



        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}

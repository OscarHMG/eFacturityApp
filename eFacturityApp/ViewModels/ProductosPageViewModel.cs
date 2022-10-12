using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Popups.ViewModels;
using eFacturityApp.Services;
using eFacturityApp.Utils;
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
    public class ProductosPageViewModel: ViewModelBase
    {
        [Reactive] public ObservableCollection<ProductoModel> ProductsList { get; set; } = new ObservableCollection<ProductoModel>();

        public ICommand LoadProductsCommand { get; set; }

        public ICommand SelectedProductCommand { get; set; }

        public ICommand NewProductCommand { get; set; }

        public ICommand DeleteProductCommand { get; set; }

        public ProductosPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {
            LoadProductsCommand = new Command(async()=> await LoadProducts());


            SelectedProductCommand = new Command<ProductoModel>(async(ProductSelected)=>
            {
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("ProductSelected", ProductSelected);
                await Navigate(_navigationService, "NewProductPage", parameters);
            });

            NewProductCommand = new Command(async()=> await Navigate(_navigationService, "NewProductPage"));

            DeleteProductCommand = new Command<ProductoModel>(async(ProductoSelected)=> await DeleteProduct(ProductoSelected));
        }

        private async Task DeleteProduct(ProductoModel productoSelected)
        {
            try
            {
                bool responsePrompt = await ShowYesNoAlert("Eliminar Producto/Servicio", "¿Está seguro que desea eliminar el producto/servicio escogido?");

                if (responsePrompt)
                {
                    await _loaderService.Show("Un momento..");
                    var response = await _apiService.DeleteProduct(productoSelected.IdProducto);
                
                    if (await HandleAPIResponse(response.statusCode, response.message, "Productos/Servicios", _navigationService))
                    {
                        LoadProductsCommand.Execute(null);
                    }
                }
            }
            catch (Exception err)
            {
                await ShowAlert("Productos", "Ocurrió un error inesperado: " + err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            LoadProductsCommand.Execute(null);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var Refresh = parameters.GetValue<bool>("Refresh");
            if (Refresh)
            {
                LoadProductsCommand.Execute(null);
            }
        }



        private async Task LoadProducts()
        {
            try
            {
                await _loaderService.Show("Consultando productos..");

                var response = await _apiService.LoadProducts();

                if (await Utility.HandleAPIResponse(response.statusCode, response.message, "Productos/Servicios", _navigationService))
                {
                    ProductsList = new ObservableCollection<ProductoModel>(response.data);
                }

                await _loaderService.Hide();

            }
            catch (Exception err)
            {
                await Utility.ShowAlert("Error - Productos", "Ocurrió un error inesperado: "+ err.Message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            }
        }
    }
}

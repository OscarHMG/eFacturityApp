using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Popups.ViewModels;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static eFacturityApp.Utils.Utility;

namespace eFacturityApp.ViewModels
{
    public class NewProductPageViewModel: ViewModelBase
    {
        [Reactive] public DropDown DropDownTipoArticulo { get; set; }

        [Reactive] public ItemPicker TipoArticuloSelected { get; set; }

        [Reactive] public DropDown DropDownImpuestos { get; set; }

        [Reactive] public ItemPicker ImpuestoSelected { get; set; }

        [Reactive] public DropDown DropDownUnidadMedidas { get; set; }

        [Reactive] public ItemPicker UnidadMedidaSelected { get; set; }

        public ICommand CreateNewProductoCommand { get; set; }
        public ICommand LoadDropDownsCommand { get; set; }

        public NewProductPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader)
        {
            LoadDropDownsCommand = new Command(() => LoadDropDowns());

            CreateNewProductoCommand = new Command(async()=> await CreateNewProduct());
        }

        private async Task CreateNewProduct()
        {
            await ShowAlert("Nuevo producto", "Su producto fue creado.",AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            await NavigateBack(_navigationService);
        
        }

        private void LoadDropDowns()
        {

            List<ItemPicker> TipoArticulos = new List<ItemPicker>()
            {
                new ItemPicker(1, "001 Bien", "001 Bien"),
                new ItemPicker(2, "002 Servicio", "001 Servicio")
            };

            List<ItemPicker> Impuestos = new List<ItemPicker>()
            {
                new ItemPicker(1, "001 IVA 12%", "001 IVA 12%"),
                new ItemPicker(2, "001 IVA 0%", "002 IVA 0%"),
                new ItemPicker(3, "003 NO OBJETO DE IVA", "003 NO OBJETO DE IVA"),
            };

            List<ItemPicker> UnidadMedidas = new List<ItemPicker>()
            {
                new ItemPicker(1, "001 Unidad", "001 Unidad"),
                new ItemPicker(2, "002 Metro", "002 Metro"),
                new ItemPicker(3, "003 KG", "003 KG"),
            };

            DropDownTipoArticulo = new DropDown(TipoArticulos);
            DropDownImpuestos = new DropDown(Impuestos);
            DropDownUnidadMedidas = new DropDown(UnidadMedidas);
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
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

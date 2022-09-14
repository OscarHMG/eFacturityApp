using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;

namespace eFacturityApp.Popups.ViewModels
{
    public class AlertFacturaItemPopupPageViewModel : ViewModelBase
    {
        public ItemFactura NewItemFactura { get; set; } = new ItemFactura();
        [Reactive] public string ErrorMessage { get; set; } = string.Empty;

        [Reactive] public DropDown DropDownArticulos { get; set; }

        [Reactive] public ItemPicker ArticuloSelected { get; set; }
        public ICommand LoadArticulosCatalogosCommand { get; set; }

        public ICommand CancelCommand { get; set; }
        public ICommand AddNewItemCommand { get; set; }



        public AlertFacturaItemPopupPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader)
        {
            LoadArticulosCatalogosCommand = new Command(() => LoadDropDowns());

            CancelCommand = new Command(async()=> await NavigateBack(_navigationService));

            AddNewItemCommand = new Command(async()=> await CreateNewItemFacturaAsync());




        }





        

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            LoadArticulosCatalogosCommand.Execute(null);
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


        private void LoadDropDowns()
        {

            List<ItemPicker> ItemsArticulos = new List<ItemPicker>()
            {
                new ItemPicker(1, "Item descripcion nombre largo", "Item descripcion nombre largo"),
                new ItemPicker(2, "Item 2", "Item 2")
            };


            DropDownArticulos = new DropDown(ItemsArticulos);
        }


        private async Task CreateNewItemFacturaAsync()
        {
            if (ArticuloSelected == null)
            {
                ErrorMessage = "Debe escoger un artículo o servicio.";
            }
            else
            {
                
                NewItemFactura.IdItem = ArticuloSelected.Id;
                NewItemFactura.NombreItem = ArticuloSelected.TextoLargo;
                NavigationParameters NavParams = new NavigationParameters();
                NavParams.Add("NewItemFactura", NewItemFactura);

                await NavigateBack(_navigationService, NavParams);
            }
        }
    }
}

using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using static eFacturityApp.Utils.Utility;

namespace eFacturityApp.ViewModels
{
    public class FacturaPageViewModel : ViewModelBase
    {
        [Reactive] public DropDown DropDownFormasPago { get; set; }

        [Reactive] public ItemPicker FormasPagoSelected { get; set; }
        [Reactive] public DropDown DropDownEstablecimientos { get; set; }

        [Reactive] public ItemPicker EstablecimientoSelected { get; set; }

        [Reactive] public DropDown DropDownPuntoVentas { get; set; }

        [Reactive] public ItemPicker PuntoVentaSelected { get; set; }

        [Reactive] public DropDown DropDownPersonas { get; set; }

        [Reactive] public ItemPicker PersonaSelected { get; set; }

        public ICommand LoadDropDownsCommand { get; set; }
        public FacturaPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader)
        {
            LoadDropDownsCommand = new Command(()=> LoadDropDowns());
        }


        private void LoadDropDowns()
        {
            
            List<ItemPicker> ItemsFormasPago = new List<ItemPicker>()
            {
                new ItemPicker(1, "Otros con la utilización del sistema financiero", "001"),
                new ItemPicker(2, "Sin utilización del sistema financiero", "002"),
                new ItemPicker(3, "Compesación de deuda", "003"),
                new ItemPicker(4, "Tarjeta de débito", "004"),
                new ItemPicker(5, "Tarjeta de crédito", "005"),
            };

            List<ItemPicker> ItemsEstablecimiento = new List<ItemPicker>()
            {
                new ItemPicker(1, "001", "001"),
            };

            List<ItemPicker> ItemsPuntoVentas = new List<ItemPicker>()
            {
                new ItemPicker(1, "001", "001"),
            };

            DropDownFormasPago = new DropDown(ItemsFormasPago);
            DropDownEstablecimientos = new DropDown(ItemsEstablecimiento);
            DropDownPuntoVentas = new DropDown(ItemsPuntoVentas);
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
    }
}

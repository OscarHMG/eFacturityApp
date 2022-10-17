using eFacturityApp.Popups.ViewModels;
using eFacturityApp.Popups.Views;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;

namespace eFacturityApp.Utils
{
    public static class Utility
    {

        public enum DOC_TYPE
        {
            FACTURA,
            LIQ_COMPRA_BIENES_SERVICIOS,
            NOTA_CREDITO,
            NOTA_DEBITO,
            COMPROBANTE_RET,
            GUIA_REMISION,
            PROFORMA
        }
        public class ItemCheckBox : INotifyPropertyChanged
        {
            public long Id { get; set; }
            
            private bool isChecked;

            private ICommand stateChangedCommand;
            public string Item { get; set; }
            public ICommand StateChangedCommand
            {
                get { return stateChangedCommand; }
                set
                {
                    stateChangedCommand = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StateChangedCommand"));
                }
            }

            public bool IsChecked
            {
                get { return isChecked; }
                set
                {
                    isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
                }
            }

            public ItemCheckBox()
            {
                StateChangedCommand = new Command(OutputValue);
            }

            void OutputValue(object person)
            {
                if (IsChecked)
                {
                    App.Current.MainPage.DisplayAlert("Message", "Selected Language", (person as SfRadioButton).Text);
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;


        }


        public class DropDown
        {
            public ObservableCollection<ItemPicker> Items { get; set; }

            [Reactive]
            public ItemPicker ItemSeleccionado { get; set; }

            public DropDown(List<ItemPicker> Items)
            {
                this.Items = new ObservableCollection<ItemPicker>(Items);
            }

            public DropDown()
            {
                this.Items = new ObservableCollection<ItemPicker>();
            }
        }


        public class ItemPicker
        {
            public long Id { get; set; }
            public string TextoLargo { get; set; }

            public string TextoCorto { get; set; }
            public ItemPicker()
            {
            }
            public ItemPicker(long Id, string TextoLargo, string TextoCorto)
            {
                this.Id = Id;
                this.TextoCorto = TextoCorto;
                this.TextoLargo = TextoLargo;
            }
        }
        public static ItemPicker SetSelectedValuesDropDown(long? IdSelected, List<ItemPicker> Options)
        {
            ItemPicker ItemSelected = new ItemPicker();
            if (IdSelected != null)
            {
                var Item = Options.FirstOrDefault(c => c.Id == IdSelected);
                if (Item != null)
                {
                    
                    ItemSelected.Id = Item.Id;
                    ItemSelected.TextoCorto = Item.TextoCorto;
                    ItemSelected.TextoLargo = Item.TextoLargo;
                }
            }
            return ItemSelected;
        }


        public static async Task ShowAlert(string Title, string Message, AlertConfirmationPopupPageViewModel.EnumInputType Type, INavigationService navigationService)
        {
            var popup = new AlertConfirmationPopupPage(Title, Message, Type);
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(popup);
            var StringResponse = await popup.PopupClosedTask;
            await navigationService.ClearPopupStackAsync();
        }


        public static async Task<bool> ShowYesNoAlert(string Title, string Message, INavigationService navigationService)
        {
            var popup = new AlertConfirmationPopupPage(Title, Message, AlertConfirmationPopupPageViewModel.EnumInputType.YesNo);
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(popup);
            var StringResponse = await popup.PopupClosedTask;
            bool response = StringResponse.Item1.ToString().ToUpper().Equals("YES") ? true : false;
            await navigationService.ClearPopupStackAsync();
            return response;
        }

        public static async Task<bool> ShowYesNoAlert(string Title, string Message)
        {
            var popup = new AlertConfirmationPopupPage(Title, Message, AlertConfirmationPopupPageViewModel.EnumInputType.YesNo);
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(popup);
            var StringResponse = await popup.PopupClosedTask;
            bool response = StringResponse.Item1.ToString().ToUpper().Equals("YES") ? true : false;
            return response;
        }



        public static async Task Navigate(INavigationService navigationService, string route, INavigationParameters parameters = null)
        {
            var Result = await navigationService.NavigateAsync(route, parameters);
            if (!Result.Success)
            {
                string URI = navigationService.GetNavigationUriPath();
                Debugger.Break();
            }
        }

        public static async Task NavigateBack(INavigationService navigationService,  INavigationParameters parameters = null)
        {
            var Result = await navigationService.GoBackAsync(parameters);
            if (!Result.Success)
            {
                Debugger.Break();
            }
        }


        public static async Task<bool> HandleAPIResponse(int statusCode, string message, string TitleModal, INavigationService navigationService)
        {
            bool success = false;
            if (statusCode == 400 || statusCode == 500)
            {
                await Utility.ShowAlert(TitleModal, message, AlertConfirmationPopupPageViewModel.EnumInputType.Ok, navigationService);
            }
            else if (statusCode == 200)
            {
                success = true;
            }

            return success;
        }
    }
}

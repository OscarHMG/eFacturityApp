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
    public class NewClientProviderPageViewModel : ViewModelBase
    {
        [Reactive] public DropDown DropDownTipoPersona { get; set; }

        [Reactive] public ItemPicker TipoPersonaSelected { get; set; }
        public ICommand CreateNewClientProviderCommand { get; set; }
        public ICommand LoadDropDownsCommand { get; set; }
        public NewClientProviderPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader)
        {

            LoadDropDownsCommand = new Command(() => LoadDropDowns());
            CreateNewClientProviderCommand = new Command(async () => await CreateNewClientProvider());

        }




        private void LoadDropDowns()
        {

            List<ItemPicker> TipoPersona = new List<ItemPicker>()
            {
                new ItemPicker(1, "NATURAL", "NATURAL"),
                new ItemPicker(2, "JURÍDICA", "JURÍDICA")
            };

            DropDownTipoPersona = new DropDown(TipoPersona);
        }
        private async Task CreateNewClientProvider()
        {
            await ShowAlert("Nuevo cliente/proveedor", "El cliente/proveedor, fue ingresado.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

            await NavigateBack(_navigationService);
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

using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Popups.ViewModels;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;
namespace eFacturityApp.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        [Reactive] public ObservableCollection<MenuItemOption> MenuOptionItems { get; set; } = new ObservableCollection<MenuItemOption>();
        [Reactive] public ICommand LoadMenuItemsOptionCommad { get; set; }
        [Reactive] public ICommand MenuOptionSelectedCommand { get; set; }

        public HomePageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader)
        {
            MenuOptionSelectedCommand = new Command<MenuItemOption>(async (OptionSelected)=>  await GoToPageSelectedAsync(OptionSelected));


            LoadMenuItemsOptionCommad = new Command(() => LoadMenuItems());
        }




        public void LoadMenuItems()
        {
            MenuOptionItems = new ObservableCollection<MenuItemOption>();

            MenuItemOption Op1 = new MenuItemOption();
            Op1.Id = 1;
            Op1.ImageIcon = "";
            Op1.Name = "Nuevo Doc. electrónico";

            MenuItemOption Op2= new MenuItemOption();
            Op2.Id = 2;
            Op2.ImageIcon = "";
            Op2.Name = "Nuevo prod/servicio";

            MenuItemOption Op3 = new MenuItemOption();
            Op3.Id = 3;
            Op3.ImageIcon = "";
            Op3.Name = "Nuevo cliente/proveedor";
            
            MenuItemOption Op4 = new MenuItemOption();
            Op4.Id = 1;
            Op4.ImageIcon = "";
            Op4.Name = "Nuevo Doc. electrónico";

            MenuItemOption Op5 = new MenuItemOption();
            Op5.Id = 1;
            Op5.ImageIcon = "";
            Op5.Name = "Nuevo Doc. electrónico";

            MenuOptionItems.Add(Op1);
            MenuOptionItems.Add(Op2);
            MenuOptionItems.Add(Op3);
            MenuOptionItems.Add(Op4);
            MenuOptionItems.Add(Op5);

        }

        private async Task GoToPageSelectedAsync(MenuItemOption Option)
        {
            switch (Option.Id)
            {
                case 1:
                    await Navigate(_navigationService, "AlertDocumentTypePopupPage");
                    break;
                case 2:
                    await Navigate(_navigationService, "ProductosPage");
                    break;
                case 3:
                    await Navigate(_navigationService, "ClientProvidersPage");
                    break;
                default:
                    await ShowAlert("Menú", "Módulo en proceso.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);

                    break;
            }
        }


        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            LoadMenuItemsOptionCommad.Execute(null);
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

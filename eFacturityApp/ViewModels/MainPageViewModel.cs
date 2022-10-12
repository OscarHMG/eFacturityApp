using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
using eFacturityApp.Utils;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace eFacturityApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ICommand ChangePasswordCommand { get; set; }

        public ICommand LogoutCommand { get; set; }
        public MainPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService)
        {
            ChangePasswordCommand = new Command(async () => {
                await Utility.Navigate(_navigationService, "/MainPage/Nav/HomePage/ChangePasswordPage");
                
            });


            LogoutCommand = new Command(async() => 
            {
                _userService.CerrarSession();
                await Utility.Navigate(_navigationService, "/Nav/LoginPage");
            });
        }
    }
}

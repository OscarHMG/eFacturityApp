using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
using eFacturityApp.Utils;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
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

        public ICommand HomeCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        public ICommand LogoutCommand { get; set; }

        public ICommand GoToProfilePageCommand { get; set; }

        [Reactive] public ICommand LoadUserInformationCommand { get; set; }
        [Reactive] public string Nombres { get; set; }
        [Reactive] public string Apellidos { get; set; }
        public MainPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService)
        {

            HomeCommand = new Command(async()=> {
                await Utility.Navigate(_navigationService, "/MainPage/Nav/HomePage");
            });
            ChangePasswordCommand = new Command(async () => {
                await Utility.Navigate(_navigationService, "/MainPage/Nav/HomePage/ChangePasswordPage");
                
            });


            LogoutCommand = new Command(async() => 
            {
                _userService.CerrarSession();
                await Utility.Navigate(_navigationService, "/Nav/LoginPage");
            });

            GoToProfilePageCommand = new Command(async()=> {
                await Utility.Navigate(_navigationService, "/MainPage/Nav/HomePage/ProfilePage");
            });

            LoadUserInformationCommand = new Command(async () =>
            {
                var user = await userService.GetUserInformationProfile();
                if (user != null)
                {
                    Nombres = user.Nombres;
                    Apellidos = user.Apellidos;
                }
            });
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            LoadUserInformationCommand.Execute(null);
        }
    }
}

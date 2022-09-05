using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
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
    public class HomePageViewModel : ViewModelBase
    {
        [Reactive] public ICommand LoginCommand { get; set; }
        public HomePageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader)
        {
            LoginCommand = new Command(async ()=> {
                var Result = await _navigationService.NavigateAsync("ChangePasswordPage");

                if (!Result.Success)
                {
                    Debugger.Break();
                }
            });
        }
        
    }
}

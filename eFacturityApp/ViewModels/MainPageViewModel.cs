using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
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
        public MainPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader)
        {
            ChangePasswordCommand = new Command(async () => {
                var Result = await _navigationService.NavigateAsync("/MainPage/Nav/HomePage/ChangePasswordPage");
                if (!Result.Success) 
                {
                    Debugger.Break();
                }
            });
        }
    }
}

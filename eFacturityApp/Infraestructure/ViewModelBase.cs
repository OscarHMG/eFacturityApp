using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Essentials;

namespace eFacturityApp.Infraestructure
{
    public class ViewModelBase : ReactiveObject, IInitialize, INavigationAware, IDestructible
    {
        protected readonly INavigationService _navigationService;
        protected readonly IPageDialogService _pageDialogService;


        protected readonly ApiService _apiService;

        protected readonly UserService _userService;
        protected LoaderService _loaderService { get; private set; }
        [Reactive] public string Title { get; set; }

        [Reactive] public bool HasInternetConnecion { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateCommand<string> NavigateCommand { get; set; }
        public static bool PopUpIsOpen { get; set; } = false;



        public ViewModelBase(INavigationService navigationService)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                //var alert = new AlertModel("Aviso", "Por favor conectese a una red", "OK");
            }
            else
            {
                _navigationService = navigationService;
                Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
                NavigateCommand = new DelegateCommand<string>(Navigate);
            }

        }


        public ViewModelBase(INavigationService navigationService, LoaderService loader, UserService userService = null, ApiService apiService = null)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.None || Connectivity.NetworkAccess == NetworkAccess.Unknown)
            {
                //var alert = new AlertModel("Aviso", "Por favor conectese a una red", "OK");
            }
            else
            {
                _navigationService = navigationService;
                _apiService = apiService;
                _userService = userService;
                loader.setNavigationService(_navigationService);
                _loaderService = loader;

                NavigateCommand = new DelegateCommand<string>(Navigate);
            }

        }


        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            HasInternetConnecion = e.NetworkAccess != NetworkAccess.Internet;
        }

        private async void Navigate(string name)
        {
            var result = await _navigationService.NavigateAsync(name);
            if (!result.Success)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
            string uri = _navigationService.GetNavigationUriPath();
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        public virtual void Destroy()
        {

        }
    }
}

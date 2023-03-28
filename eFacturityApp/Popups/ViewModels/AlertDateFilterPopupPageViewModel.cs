using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;
using static Xamarin.Forms.Internals.Profile;

namespace eFacturityApp.Popups.ViewModels
{
    public class AlertDateFilterPopupPageViewModel : ViewModelBase
    {
        [Reactive] public DateTime Desde { get; set; }

        [Reactive] public DateTime Hasta { get; set; }

        [Reactive] public string ErrorMessage { get; set; }

        [Reactive] public ICommand ApplyFiltersCommand { get; set; }

        [Reactive] public ICommand CancelCommand { get; set; }

        [Reactive] public ICommand OpenFilterPopUpCommand { get; set; }
        public AlertDateFilterPopupPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader)
        {
            CancelCommand = new Command(async () => await NavigateBack(_navigationService));

            ApplyFiltersCommand = new Command(async () =>
            {
                int months = (Hasta.Year - Desde.Year) * 12 + Hasta.Month - Desde.Month;
                if (months > 1)
                {
                    ErrorMessage = "Solamente puede haber un mes de diferencia.";
                }
                else
                {
                    NavigationParameters parameters = new NavigationParameters
                    {
                        { "Desde", Desde },
                        { "Hasta", Hasta },
                        { "ApplyFilters", true}
                    };
                    await NavigateBack(_navigationService, parameters);
                }
            });



        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            Desde = DateTime.Now.Date;
            Hasta = DateTime.Now.Date;
            Title = parameters.GetValue<string>("title") ?? "Filtro Fecha - Documento";
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

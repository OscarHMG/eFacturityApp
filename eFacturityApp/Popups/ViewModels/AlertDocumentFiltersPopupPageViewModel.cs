using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using static eFacturityApp.Utils.Utility;

namespace eFacturityApp.Popups.ViewModels
{
    public class AlertDocumentFiltersPopupPageViewModel : ViewModelBase
    {

        [Reactive] public string Estado { get; set; }
        [Reactive] public string Codigo { get; set; }
        [Reactive] public long? IdTipoDocumento { get; set; }
        [Reactive] public long? IdPersona { get; set; }

        [Reactive] public DropDown DropDownTipoEstado { get; set; }

        [Reactive] public ItemPicker EstadoSelected { get; set; }

        [Reactive] public DropDown DropDownPersonas { get; set; }

        [Reactive] public ItemPicker PersonaSelected { get; set; }


        

        [Reactive] public ICommand ApplyFiltersCommand { get; set; }

        [Reactive] public ICommand CancelCommand { get; set; }
        public AlertDocumentFiltersPopupPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader, userService, apiService)
        {
            

            CancelCommand = new Command(async () => await NavigateBack(_navigationService));

            ApplyFiltersCommand = new Command(async () => 
            {
                NavigationParameters parameters = new NavigationParameters();
                FiltersApiModel filtros = new FiltersApiModel();
                filtros.Codigo = Codigo;
                filtros.IdPersona = PersonaSelected?.Id;
                filtros.IdTipoDocumento = 2;
                filtros.Estado = EstadoSelected?.TextoLargo;
                parameters.Add("Filtros", filtros);

                await NavigateBack(_navigationService, parameters);
            });
        }

        

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            var Estados = parameters.GetValue<List<ItemPicker>>("Estados");
            var ItemsPersonas = parameters.GetValue<List<ItemPicker>>("Personas");
            var Filters = parameters.GetValue<FiltersApiModel>("FiltersSelected");
            if (Estados != null)
            {
                DropDownTipoEstado = new DropDown(Estados);
            }

            if (ItemsPersonas != null)
            {
                DropDownPersonas = new DropDown(ItemsPersonas);
            }

            if (Filters != null)
            {
                Estado = Filters.Estado;
                EstadoSelected = DropDownTipoEstado.Items.FirstOrDefault(c=> c.TextoLargo == Filters.Estado );
                IdPersona = Filters.IdPersona;
                PersonaSelected = DropDownPersonas.Items.FirstOrDefault(c=> c.Id == Filters.IdPersona);
                Codigo = Filters.Codigo;
            }
            
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

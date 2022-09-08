using DynamicData;
using eFacturityApp.Infraestructure;
using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Popups.Views;
using eFacturityApp.Services;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static eFacturityApp.Utils.Utility;

namespace eFacturityApp.Popups.ViewModels
{
    public class AlertDocumentTypePopupPageViewModel : ViewModelBase
    {

        [Reactive] public ObservableCollection<ItemCheckBox> Items { get; set; } = new ObservableCollection<ItemCheckBox>();

        [Reactive] public ItemCheckBox DocumentTypeChecked { get; set; }

        
        public ICommand LoadDocumentTypesCommand { get; set; }

        public ICommand DocumentTypeSelectedCommand { get; set; }


        public ICommand OptionSelectedCommand { get; set; }

        public ICommand CancelCommand { get; set; }
        [Reactive] public string ErrorMessage { get; set; }

        public AlertDocumentTypePopupPageViewModel(INavigationService navigationService, LoaderService loader, UserService userService, ApiService apiService) : base(navigationService, loader)
        {
            
            LoadDocumentTypesCommand = new Command(()=> LoadDocumentTypes());


            DocumentTypeSelectedCommand = new Command(async() => await GoToDocumentTypeFormAsync());

            CancelCommand = new Command(async()=> await NavigateBack(_navigationService));
        }

        

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            LoadDocumentTypesCommand.Execute(null);
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

        private async Task GoToDocumentTypeFormAsync()
        {
            DocumentTypeChecked = Items.FirstOrDefault(c => c.IsChecked);
            if (DocumentTypeChecked != null)
            {
                ErrorMessage = string.Empty;
                switch (DocumentTypeChecked.Id)
                {
                    case (long)DOC_TYPE.FACTURA:
                        await Navigate(_navigationService, "FacturaPage");
                        break;
                    default:
                        await ShowAlert("Tipo de Documento", "Trabajo en proceso.", AlertConfirmationPopupPageViewModel.EnumInputType.Ok, _navigationService);
                        break;
                }
            }
            else
            {
                ErrorMessage = "(*) Debe escoger un documento.";
            }
        }

        private void LoadDocumentTypes()
        {
            Items = new ObservableCollection<ItemCheckBox>();
            List<ItemCheckBox> DocTypes = new List<ItemCheckBox>()
            {
                new ItemCheckBox(){Id= 0, Item = "Factura" , IsChecked = false},
                new ItemCheckBox(){Id= 2,Item = "Liquidación de compra de bienes o servicios" , IsChecked = false},
                new ItemCheckBox(){Id= 3,Item = "Notas de crédito" , IsChecked = false},
                new ItemCheckBox(){Id= 4,Item = "Nota de débito" , IsChecked = false},
                new ItemCheckBox(){Id= 5,Item = "Comprobante de retención" , IsChecked = false},
                new ItemCheckBox(){Id= 6,Item = "Guía de remisión" , IsChecked = false},
                new ItemCheckBox(){Id= 7,Item = "Proforma" , IsChecked = false},
            };

            Items.Add(DocTypes);
        }
    }
}

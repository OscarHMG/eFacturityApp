using eFacturityApp.Infraestructure;
using Prism.Navigation;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace eFacturityApp.Popups.ViewModels
{
    public class AlertPopupPageViewModel : ViewModelBase
    {
        [Reactive] public string TitleAlert { get; set; }
        [Reactive] public string Message { get; set; }

        [Reactive] public string ButtonText { get; set; }

        public ICommand ButtonActionCommand { get; set; }

        [Reactive] public bool BackAfterPopIsClose { get; set; }

        public AlertPopupPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            ButtonActionCommand = new Command(async () =>
            {
                //Para no olvidarme: Si parto desde la Actividad A y quiero que despues de mostrar este
                //popup la actividad A, se vaya hacia atras la actividad me pasa el bool en true y el popup al regresar
                //le indica a la actividad A que debe irse hacia atras (Ejemplo Registro exitoso -> Muestra el PopUp y al
                //cerrar el popup la actividad del registro regresa hacia el
                //in.)
                NavigationParameters Params = new NavigationParameters();
                Params.Add("BackAfterPopUpIsClose", BackAfterPopIsClose);
                await navigationService.GoBackAsync(Params);
            });
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            //var Params = parameters.GetValue<AlertModel>("Alert");
            //BackAfterPopIsClose = parameters.GetValue<bool>("BackAfterPopUpIsClose");
            //if (Params != null)
            //{
            //    TitleAlert = Params.Title;
            //    Message = Params.Message;
            //    ButtonText = Params.ButtonText;
            //}
        }
    }
}

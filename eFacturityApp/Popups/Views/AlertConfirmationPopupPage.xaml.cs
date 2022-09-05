using eFacturityApp.Popups.ViewModels;
using eFacturityApp.ViewModels;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eFacturityApp.Popups.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlertConfirmationPopupPage : PopupPage
    {
        public AlertConfirmationPopupPage()
        {
            InitializeComponent();
        }

        private TaskCompletionSource<Tuple<AlertConfirmationPopupPageViewModel.EnumOutputType, string>> _taskCompletionSource;
        public Task<Tuple<AlertConfirmationPopupPageViewModel.EnumOutputType, string>> PopupClosedTask => _taskCompletionSource.Task;


        public AlertConfirmationPopupPage(string title, string message, AlertConfirmationPopupPageViewModel.EnumInputType inputType)
        {
            InitializeComponent();
            BindingContext = new AlertConfirmationPopupPageViewModel(title, message, inputType);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _taskCompletionSource = new TaskCompletionSource<Tuple<AlertConfirmationPopupPageViewModel.EnumOutputType, string>>();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _taskCompletionSource.SetResult(((AlertConfirmationPopupPageViewModel)BindingContext).ReturnValue);
        }
    }
}
using eFacturityApp.Popups.ViewModels;
using eFacturityApp.Popups.Views;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace eFacturityApp.Utils
{
    public static class Utility
    {

        public static async Task ShowAlert(string Title, string Message, AlertConfirmationPopupPageViewModel.EnumInputType Type, INavigationService navigationService)
        {
            var popup = new AlertConfirmationPopupPage(Title, Message, Type);
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(popup);
            var StringResponse = await popup.PopupClosedTask;
            await navigationService.ClearPopupStackAsync();
        }

        public static async Task Navigate(INavigationService navigationService, string route)
        {
            var Result = await navigationService.NavigateAsync(route);
            if (!Result.Success)
            {
                Debugger.Break();
            }
        }
    }
}

using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eFacturityApp.Services
{
    public class LoaderService
    {
        private INavigationService NavigationService;
        public LoaderService()
        {

        }

        public void setNavigationService(INavigationService _navigationService)
        {
            NavigationService = _navigationService;
        }

        public async Task Show(string text)
        {
            NavigationParameters nav = new NavigationParameters();
            nav.Add("LoadingText", text);
            var result = await NavigationService.NavigateAsync("LoaderPopup", nav);
            if (!result.Success)
            {
                System.Diagnostics.Debugger.Break();
            }
        }
        public async Task Hide()
        {
            var result = await NavigationService.ClearPopupStackAsync(animated: true);
            if (!result.Success)
                System.Diagnostics.Debugger.Break();
        }
        public async Task HideAndNavigate(string location)
        {
            INavigationResult result;
            await this.Hide();
            result = await NavigationService.NavigateAsync($"{location}");
            if (!result.Success)
            {
                Console.WriteLine(result.Exception.Message);
            }
        }

        public async Task HideAndGoBack()
        {
            INavigationResult result;
            await this.Hide();
            result = await NavigationService.GoBackAsync();
            if (!result.Success)
            {
                Console.WriteLine(result.Exception.Message);
            }
        }


    }
}

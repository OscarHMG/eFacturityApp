using eFacturityApp.ViewModels;
using eFacturityApp.Views;
using Prism;
using Prism.Ioc;
using System.Diagnostics;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace eFacturityApp
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzQ2Mzk5QDMxMzgyZTMzMmUzMGFwRnBJZkZzNnp1TXBjNzl4OEM2M01SL2tyTlB3WHJQa0dHSXl1R2txaVE9");

            var Result = await NavigationService.NavigateAsync("/MainPage/Nav/HomePage");
            if (!Result.Success)
            {
                Debugger.Break();
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>("Nav");

            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>("LoginPage");
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>("HomePage");
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<ChangePasswordPage>("ChangePasswordPage");
            
        }
    }
}

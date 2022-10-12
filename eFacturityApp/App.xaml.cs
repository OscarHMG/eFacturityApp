using eFacturityApp.Infraestructure.Services;
using eFacturityApp.Popups.ViewModels;
using eFacturityApp.Popups.Views;
using eFacturityApp.Services;
using eFacturityApp.Utils;
using eFacturityApp.ViewModels;
using eFacturityApp.Views;
using Prism;
using Prism.Ioc;
using Prism.Plugin.Popups;
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

            UserService userService = new UserService(NavigationService, null);
            string PathToNavigate = await userService.HasValidToken() ? "/MainPage/Nav/HomePage" : "/Nav/LoginPage";
            await Utility.Navigate(NavigationService, PathToNavigate);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterPopupNavigationService();
            containerRegistry.RegisterPopupDialogService();
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.RegisterSingleton<ApiService>();
            containerRegistry.RegisterSingleton<UserService>();
            containerRegistry.RegisterSingleton<LoaderService>();

            //POPUP PAGES
            containerRegistry.RegisterForNavigation<LoaderPopupPage, LoaderPopupPageViewModel>("LoaderPopupPage");
            containerRegistry.RegisterForNavigation<AlertDocumentTypePopupPage, AlertDocumentTypePopupPageViewModel>("AlertDocumentTypePopupPage");


            containerRegistry.RegisterForNavigation<NavigationPage>("Nav");

            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>("LoginPage");
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>("HomePage");
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<ChangePasswordPage>("ChangePasswordPage");
            containerRegistry.RegisterForNavigation<RegisterPage, RegisterPageViewModel>("RegisterPage");
            containerRegistry.RegisterForNavigation<RecoverAccountPage, RecoveryAccountPageViewModel>("RecoverAccount");
            containerRegistry.RegisterForNavigation<FacturaPage, FacturaPageViewModel>("FacturaPage");
            containerRegistry.RegisterForNavigation<AlertFacturaItemPopupPage, AlertFacturaItemPopupPageViewModel>("AlertFacturaItemPopupPage");
            containerRegistry.RegisterForNavigation<NewProductPage, NewProductPageViewModel>("NewProductPage");
            containerRegistry.RegisterForNavigation<NewClientProviderPage, NewClientProviderPageViewModel>("NewClientProviderPage");
            containerRegistry.RegisterForNavigationOnIdiom<ProductosPage, ProductosPageViewModel>("ProductosPage");
            containerRegistry.RegisterForNavigation<ClientProvidersPage, ClientProvidersPageViewModel>("ClientProvidersPage");
        }
    }
}

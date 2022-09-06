using eFacturityApp.Infraestructure;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace eFacturityApp.Popups.ViewModels
{
    public class LoaderPopupPageViewModel : ViewModelBase
    {
        public LoaderPopupPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            Title = parameters.GetValue<string>("LoadingText");
        }
    }
}

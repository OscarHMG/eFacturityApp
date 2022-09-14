using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static eFacturityApp.Utils.Utility;
namespace eFacturityApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseNavigationBarPage : ContentPage
    {
        /// <summary>
        /// Gets or Sets the Back button click overriden custom action
        /// </summary>
        public Action CustomBackButtonAction { get; set; }

        public static readonly BindableProperty EnableBackButtonOverrideProperty =
               BindableProperty.Create(
               nameof(EnableBackButtonOverride),
               typeof(bool),
               typeof(BaseNavigationBarPage),
               false);

        /// <summary>
        /// Gets or Sets Custom Back button overriding state
        /// </summary>
        public bool EnableBackButtonOverride
        {
            get
            {
                return (bool)GetValue(EnableBackButtonOverrideProperty);
            }
            set
            {
                SetValue(EnableBackButtonOverrideProperty, value);
            }
        }


        public BaseNavigationBarPage()
        {
            InitializeComponent();


            if (EnableBackButtonOverride)
            {
                this.CustomBackButtonAction = async () =>
                {
                    var Response = await ShowYesNoAlert("", "¿Está seguro que desea salir sin guardar los cambios?");

                    if (Response)
                    {
                        await Navigation.PopAsync(true);
                    }
                };
            }


        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

            if (EnableBackButtonOverride)
            {
                var Response = await ShowYesNoAlert("", "¿Está seguro que desea salir sin guardar los cambios?");
                if (Response)
                {
                    await Navigation.PopAsync(true);
                }
            }
            else
            {
                await Navigation.PopAsync(true);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using MasterDetailPage = Xamarin.Forms.MasterDetailPage;

namespace eFacturityApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();
            var safeInsets = On<iOS>().SafeAreaInsets();
            safeInsets.Top = 20;
            Padding = safeInsets;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            LOL.IsPresented = false;
        }
    }
}
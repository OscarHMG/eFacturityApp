using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eFacturityApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            var master = App.Current.MainPage as MasterDetailPage;
            if (master == null) return;
            master.IsPresented = true;
        }
    }
}
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
    public partial class LiquidacionCompraPage : BaseNavigationBarPage
    {
        public LiquidacionCompraPage()
        {
            InitializeComponent();
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            Entry SenderEntry = sender as Entry;
            if (SenderEntry != null)
            {
                decimal Valor = string.IsNullOrEmpty(SenderEntry.Text) ? 0 : decimal.Parse(SenderEntry.Text);
                MessagingCenter.Send<LiquidacionCompraPage, decimal>(this, "ValorDesc", Valor);
            }

        }
    }
}
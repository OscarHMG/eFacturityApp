using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace eFacturityApp.Views
{
    public partial class ProformaPage : BaseNavigationBarPage
    {
        public ProformaPage()
        {
            InitializeComponent();
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            Entry SenderEntry = sender as Entry;
            if (SenderEntry != null)
            {
                decimal Valor = string.IsNullOrEmpty(SenderEntry.Text) ? 0 : decimal.Parse(SenderEntry.Text);
                MessagingCenter.Send<ProformaPage, decimal>(this, "ValorDesc", Valor);
            }

        }
    }
}


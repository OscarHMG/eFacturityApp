using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace eFacturityApp.Infraestructure.ApiModels
{
    public class APIModels
    {

        public class MenuItemOption
        {
            public long Id { get; set; }
            public string Name { get; set; }

            public string ImageIcon { get; set; }

        }


        #region Factura
        public class Factura
        {
            public DateTime FechaEmision { get; set; }
            public int DiasCredito { get; set; }

            public long IdFormaPago { get; set; }
            public long IdEstablecimiento { get; set; }

            public long IdPuntoVenta { get; set; }

            public long IdPersona { get; set; }

            public string Vendedor { get; set; }

            public string ComentariosFactura { get; set; }


            public List<ItemFactura> ItemsFactura { get; set; }

            public Factura(DateTime fechaEmision, int diasCredito, long idFormaPago, long idEstablecimiento, long idPuntoVenta, long idPersona, string vendedor, string comentariosFactura)
            {
                FechaEmision = fechaEmision;
                DiasCredito = diasCredito;
                IdFormaPago = idFormaPago;
                IdEstablecimiento = idEstablecimiento;
                IdPuntoVenta = idPuntoVenta;
                IdPersona = idPersona;
                Vendedor = vendedor;
                ComentariosFactura = comentariosFactura;
                ItemsFactura = new List<ItemFactura>();
            }

            public Factura()
            {
                ItemsFactura = new List<ItemFactura>();
            }
        }


        public class ItemFactura
        {
            public string GUID { get; set; }
            public int Cantidad { get; set; } = 1;

            public long IdItem { get; set; }

            public string NombreItem { get; set; }

            public decimal PrecioUnitario { get; set; }

            public decimal Descuento { get; set; }

            public decimal Subtotal { get; set; }

            public ItemFactura()
            {
                GUID = Guid.NewGuid().ToString();
            }
        }
        #endregion


    }
}

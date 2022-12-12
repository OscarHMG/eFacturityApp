using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;

namespace eFacturityApp.DataTemplateSelectors
{
    public class LiquidacionCompraSelector : DataTemplateSelector
    {
        public DataTemplate EmitidoTemplate { get; set; } // VERDE

        public DataTemplate PendienteEnvioSRITemplate { get; set; } // NARANJA

        public DataTemplate AnuladoTemplate { get; set; } // ROJO

        public DataTemplate CobradoTemplate { get; set; } // MORADO

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var LiquidacionCompraItem = (LiquidacionCompraModel)item;
            //EMITIDO
            if ((LiquidacionCompraItem.DocAutorizado != null && LiquidacionCompraItem.DocAutorizado.Value == true) && LiquidacionCompraItem.DocAnulado == false && LiquidacionCompraItem.DocCobrado == false)
            {
                return EmitidoTemplate;
            }
            //PENDIENTES
            else if ((LiquidacionCompraItem.DocAutorizado == null || LiquidacionCompraItem.DocAutorizado == false) && LiquidacionCompraItem.DocAnulado == false && LiquidacionCompraItem.DocCobrado == false)
            {
                return PendienteEnvioSRITemplate;
            }
            //ANULADO
            else if (LiquidacionCompraItem.DocAnulado == true)
            {
                return AnuladoTemplate;
            }
            //COBRADOS
            else if (LiquidacionCompraItem.DocCobrado == true)
            {
                return CobradoTemplate;
            }
            return null;
        }
    }
}

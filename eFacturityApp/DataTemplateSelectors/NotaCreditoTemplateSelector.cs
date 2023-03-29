using System;
using System.Collections.Generic;
using System.Text;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using Xamarin.Forms;

namespace eFacturityApp.DataTemplateSelectors
{
    public class NotaCreditoTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EmitidoTemplate { get; set; } // VERDE

        public DataTemplate PendienteEnvioSRITemplate { get; set; } // NARANJA

        public DataTemplate AnuladoTemplate { get; set; } // ROJO

        public DataTemplate CobradoTemplate { get; set; } // MORADO

        //public DataTemplate DefaultTemplate 

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var NotaCreditoItem = (NotaCreditoModel)item;
            //EMITIDO
            if ((NotaCreditoItem.DocAutorizado != null && NotaCreditoItem.DocAutorizado.Value == true) && NotaCreditoItem.DocAnulado == false && NotaCreditoItem.DocCobrado == false)
            {
                return EmitidoTemplate;
            }
            //PENDIENTES
            else if ((NotaCreditoItem.DocAutorizado == null || NotaCreditoItem.DocAutorizado == false) && NotaCreditoItem.DocAnulado == false && NotaCreditoItem.DocCobrado == false)
            {
                return PendienteEnvioSRITemplate;
            }
            //ANULADO
            else if (NotaCreditoItem.DocAnulado == true)
            {
                return AnuladoTemplate;
            }
            //COBRADOS
            else if (NotaCreditoItem.DocCobrado == true)
            {
                return CobradoTemplate;
            }

            return null;
        }
    }
}

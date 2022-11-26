using System;
using System.Collections.Generic;
using System.Text;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;
using Xamarin.Forms;

namespace eFacturityApp.DataTemplateSelectors
{
    public class ProformaTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EmitidoTemplate { get; set; } // VERDE

        public DataTemplate PendienteEnvioSRITemplate { get; set; } // NARANJA

        public DataTemplate AnuladoTemplate { get; set; } // ROJO

        public DataTemplate CobradoTemplate { get; set; } // MORADO

        //public DataTemplate DefaultTemplate 

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var FacturaItem = (FacturaModel)item;
            //EMITIDO
            if ((FacturaItem.DocAutorizado != null && FacturaItem.DocAutorizado.Value == true) && FacturaItem.DocAnulado == false && FacturaItem.DocCobrado == false)
            {
                return EmitidoTemplate;
            }
            //PENDIENTES
            else if ((FacturaItem.DocAutorizado == null || FacturaItem.DocAutorizado == false) && FacturaItem.DocAnulado == false && FacturaItem.DocCobrado == false)
            {
                return PendienteEnvioSRITemplate;
            }
            //ANULADO
            else if (FacturaItem.DocAnulado == true)
            {
                return AnuladoTemplate;
            }
            //COBRADOS
            else if (FacturaItem.DocCobrado == true)
            {
                return CobradoTemplate;
            }

            return null;
        }
    }
}

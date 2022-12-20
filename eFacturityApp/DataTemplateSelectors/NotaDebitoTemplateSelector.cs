using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;

namespace eFacturityApp.DataTemplateSelectors
{
    public class NotaDebitoTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EmitidoTemplate { get; set; } // VERDE

        public DataTemplate PendienteEnvioSRITemplate { get; set; } // NARANJA

        public DataTemplate AnuladoTemplate { get; set; } // ROJO

        public DataTemplate CobradoTemplate { get; set; } // MORADO

        //public DataTemplate DefaultTemplate 

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var NotaDebito = (NotaDebitoModel)item;
            //EMITIDO
            if ((NotaDebito.DocAutorizado != null && NotaDebito.DocAutorizado.Value == true) && NotaDebito.DocAnulado == false && NotaDebito.DocCobrado == false)
            {
                return EmitidoTemplate;
            }
            //PENDIENTES
            else if ((NotaDebito.DocAutorizado == null || NotaDebito.DocAutorizado == false) && NotaDebito.DocAnulado == false && NotaDebito.DocCobrado == false)
            {
                return PendienteEnvioSRITemplate;
            }
            //ANULADO
            else if (NotaDebito.DocAnulado == true)
            {
                return AnuladoTemplate;
            }
            //COBRADOS
            else if (NotaDebito.DocCobrado == true)
            {
                return CobradoTemplate;
            }

            return null;
        }
    }
}

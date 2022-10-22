﻿using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace eFacturityApp.Infraestructure.ApiModels
{
    public class APIModels
    {
        #region BaseResponse DATA
        public class GenericResponseObject<T>
        {
            private T _data;
            private bool _success = true;
            private string _message;
            private int _statusCode { get; set; }
            public long IdUsuario { get; set; }

            [DataMember()]
            public int statusCode
            {
                get { return _statusCode; }
                set { _statusCode = value; }
            }
            [DataMember()]
            public bool success
            {
                get { return _success; }
                set { _success = value; }
            }
            [DataMember()]
            public string message
            {
                get { return _message; }
                set { _message = value; }
            }

            [DataMember()]
            public T data
            {
                get { return _data; }
                set { _data = value; }
            }

            //public RecordResponseObject(ref T data)
            //{
            //    _data = data;
            //}
        }
        #endregion


        #region Request Models API
        public class LoginUsuarioRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }

            public LoginUsuarioRequest(string username, string password)
            {
                Username = username;
                Password = password;
            }

            public LoginUsuarioRequest()
            {

            }
        }

        public class ChangePasswordAPIModel
        {
            public string Email { get; set; }
            public string Password { get; set; }

            public ChangePasswordAPIModel()
            {

            }

            public ChangePasswordAPIModel(string email, string password)
            {
                Email = email;
                Password = password;
            }
        }

        #endregion

        #region Responses API
        public class PerfilUsuarioResponse
        {
            public long IdUsuario { get; set; }
            public string RazonSocial { get; set; }
            public string Apellido { get; set; }
            public string RUC { get; set; }
            public string Token { get; set; }
            public string Usuario { get; set; }
            public string Clave { get; set; }

            public string Correo { get; set; }
        }

        public class CatalogosProductoModel
        {
            public List<TipoArticuloModel> TiposArticulos { get; set; }

            public CatalogosProductoModel()
            {
                TiposArticulos = new List<TipoArticuloModel>();
                TiposImpuestos = new List<TipoImpuestoModel>();
                UnidadesMedida = new List<UnidadMedidaModel>();
            }

            public List<TipoImpuestoModel> TiposImpuestos { get; set; }
            public List<UnidadMedidaModel> UnidadesMedida { get; set; }
        }


        public class TipoArticuloModel
        {
            public long IdTipoArticulo { get; set; }
            public string CodigoArticulo { get; set; }
            public string Nombre { get; set; }
        }
        public class TipoImpuestoModel
        {
            public long IdImpuesto { set; get; }
            public string Codigo { set; get; }
            public string Nombre { set; get; }
            public decimal? Tarifa { set; get; }
        }
        public class UnidadMedidaModel
        {
            public long IdUnidadMedida { set; get; }
            public string Codigo { set; get; }
            public string Nombre { set; get; }

            public long? IdEmpresa { set; get; }
        }


        public class PerfilUsuarioModel
        {
            public long IdUsuario { set; get; }
            public long IdTipoPersona { set; get; }
            public string Ruc { set; get; }
            public string RazonSocial { set; get; }
            public string NombreComercial { set; get; }
            public string NumeroEstablecimientos { set; get; }
            public string NumeroPuntoEmision { set; get; }
            public bool ObligadollevarContabilidad { set; get; }
            public bool EsContribuyenteEspecial { set; get; }
            public string NumerodeResolucion { set; get; }
            public long? IdTipoExportador { set; get; }
            public bool EsContribuyenteRimpe { set; get; }
            public long? IdTipoContribuyenteRimpe { set; get; }
            public bool AgenteRetencíon { set; get; }
            public string Ciudad { set; get; }
            public string Movil { set; get; }
            public string Telefono { set; get; }
            public string Correo { set; get; }
            public string DireccionDomiciaria { set; get; }
            public long? IdActividadEconomica { set; get; }
            public string LogoEmpresa { set; get; }
            public string ActividadEconomica { set; get; }
            public int? NumDecimales { set; get; }

            public string RutaImagen { set; get; }
            public string NombreImagen { set; get; }
            public long? IdTipoPlan { set; get; }
            public DateTime? FechaInicioPlan { set; get; }
            public DateTime? FechaFinalPlan { set; get; }
            public string NombrePlan { set; get; }

            public string Nombres { get; set; }
            public string Apellidos { get; set; }
        }


        public class ProductoModel
        {
            public long IdProducto { set; get; }
            public long IdTipoArticulo { set; get; }
            public long? IdCategoria { set; get; }
            public long? IdUnidadMedida { set; get; }
            public string NombredUnidadMedida { set; get; }
            public long IdImpuesto { set; get; }
            public long IdEmpresa { set; get; }
            public string NombreImpuesto { set; get; }
            public string Codigo { set; get; }
            public string Nombre { set; get; }
            public decimal Precio { set; get; }

            public decimal StrinPrecio { set; get; }

            public string CodigoAuxiliar { set; get; }
            public string Descripcion { set; get; }
            public string ProductoIva { set; get; }
            public int? NumDecimales { set; get; }
            public bool? PrecioManual { set; get; }



        }



        public class PersonaModel
        {
            public long IdPersona { set; get; }
            public long IdEmpresa { set; get; }
            public long IdTipoTipoPersona { set; get; }
            public string NombreTipoTipoPersona { set; get; }
            public bool EsContribuyenteEspecial { set; get; }
            public string Ruc { set; get; }
            public string RazonSocial { set; get; }
            public string NombreComercial { set; get; }
            public string Direccion { set; get; }
            public string Telefono { set; get; }
            public bool Extranjero { set; get; }
            public string Correo { set; get; }
            public string Correo2 { set; get; }
            public string Usuario { set; get; }
            public long? IdUsuario { set; get; }
            public long? IdRegimen { set; get; }
            public string NombreRegimen { set; get; }
            public bool? EsAgenteRetencion { set; get; }
        }

        public class CatalogosApiModel
        {
            public CatalogosApiModel()
            {
                Personas = new List<PersonaModel>();
                FormasPago = new List<FormaPagoModel>();
                Establecimientos = new List<EstablecimientoModel>();
                PuntosVenta = new List<PuntoVentaModel>();
                Productos = new List<ProductoModel>();
            }
            public List<PersonaModel> Personas { get; set; }
            public List<FormaPagoModel> FormasPago { get; set; }
            public List<EstablecimientoModel> Establecimientos { get; set; }
            public List<PuntoVentaModel> PuntosVenta { get; set; }
            public List<ProductoModel> Productos { get; set; }
        }

        public class FormaPagoModel
        {
            public long IdFormaPago { set; get; }
            public string Codigo { set; get; }
            public string Nombre { set; get; }
        }
        public class EstablecimientoModel
        {
            public long IdEstablecimiento { set; get; }
            public long? IdEmpresa { set; get; }
            public string Nombre { set; get; }
            public string Codigo { set; get; }
            public string Direccion { set; get; }
            public long? IdUsuario { set; get; }
        }
        public class PuntoVentaModel
        {
            public long IdPuntoVenta { set; get; }
            public long IdEstablecimiento { set; get; }

            public string Nombre { set; get; }
            public string Codigo { set; get; }

            public long? IdUsuario { set; get; }
            public long? IdEmpresa { set; get; }
        }


        #region FACTURA
        public class FacturaModel
        {
            public FacturaModel()
            {
                Items = new List<ItemFacturaModel>();
                ErroresDocumentos = new List<ErroresDocumentos>();
                FechaEmision = DateTime.Now;
                DiasCredito = 0;
                PorcentajeDescuento = 0;
            }
            public List<ItemFacturaModel> Items { get; set; }
            public List<ErroresDocumentos> ErroresDocumentos { get; set; }
            public long? IdDocumentoCabecera { set; get; }
            public long IdTipoDocumento { set; get; }
            public string TipoDocumento { set; get; }
            public long? IdEmpresa { set; get; }
            public string NombreEmpresa { set; get; }
            public long? IdEstablecimiento { set; get; }
            public string CodigoEstablecimiento { set; get; }
            public long? IdPuntoVenta { set; get; }
            public string CodigoPuntoVenta { set; get; }
            public DateTime FechaEmision { set; get; }
            public DateTime FechaRegistro { set; get; }
            public DateTime? FechaAnulacion { set; get; }
            public DateTime? FechaCobro { set; get; }
            public bool? DocXmlGenerado { set; get; }
            public bool? DocFirmado { set; get; }
            public bool? DocRecibido { set; get; }
            public bool? DocAutorizado { set; get; }
            public bool? DocRide { set; get; }
            public bool? DocEnviar { set; get; }
            public bool DocAnulado { set; get; }
            public bool DocCobrado { set; get; }
            public string FechaEmision2 { set; get; }

            public long IdPersona { set; get; }
            public string NombrePersona { set; get; }
            public DateTime FechaVencimiento { set; get; }
            public string DireccionMatriz { set; get; }
            public string DireccionSucursal { set; get; }

            public int NumDocumento { set; get; }

            public string Info1Direccion { set; get; }

            public string Info2Email { set; get; }

            public long? IdFormaPago { set; get; }

            public string NombreFormaPago { set; get; }
            public string NombreTipoDocumento { set; get; }
            //public long? IdTipoDocumentoModificado { set; get; }
            //public string ComprobanteModifica { set; get; }
            //public string RazonModificacion { set; get; }

            public string ClaveAccesoXML { set; get; }

            public string Secuencial { set; get; }

            public decimal ComprobantevSubtotal0 { set; get; }

            public decimal ComprobantevTotal { set; get; }

            public string Estado { set; get; }
            public string Descripcion { set; get; }
            public decimal ComprobantevSubtotal { get; set; }
            public decimal ComprobantevIvatotal { get; set; }

            public decimal ComprobantevICEtotal { get; set; }
            public string Mensaje { get; set; }
            public bool Exitoso { get; set; }
            public long? DiasCredito { set; get; }
            public string CodVendedor { set; get; }

            public string NombreVendedor { get; set; }

            public decimal? PorcentajeDescuento { set; get; }
            public decimal TotalDescuento { set; get; }

            public int NumeroDecimales { set; get; }
            public string MensajeErrorDocXmlGenerado { set; get; }
            public string MensajeErrorocFirmado { set; get; }
            public string MensajeErrorDocRecibido { set; get; }
            public string MensajeErrorDocAutorizado { set; get; }
            public string MensajeErrorDocRide { set; get; }
            public string MensajeErrorCorreoEnviado { set; get; }
        }

        public class ItemFacturaModel
        {
            public long? IdDocumentoCabecera { set; get; }
            public decimal Cantidad { get; set; } = 1;
            public decimal Subtotal { set; get; }
            public decimal Total { set; get; }
            [Reactive] public decimal Descuento { set; get; }
            public decimal ValorDescuento { set; get; }
            public decimal Ivatotal { set; get; }
            public decimal Precio { set; get; }

            public bool? PrecioManual { get; set; }

            //public decimal? PrecioManual { set; get; }
            public string Descripcion { set; get; }
            public string NombreProducto { set; get; }
            public long IdProducto { set; get; }

            public string GUID { get; set; } = Guid.NewGuid().ToString();
        }

        public class ErroresDocumentos
        {
            public string Error { get; set; }
            public string TipoError { get; set; }
        }

        public class ListarDocumentosGeneradosModel
        {
            public ListarDocumentosGeneradosModel()
            {
                Documentos = new List<FacturaModel>();
            }
            public List<FacturaModel> Documentos;
        }


        public class FiltersApiModel
        {
            public DateTime? Desde { get; set; }
            public DateTime? Hasta { get; set; }
            public string Estado { get; set; }
            public string Codigo { get; set; }
            public long? IdTipoDocumento { get; set; }
            public long? IdPersona { get; set; }

            public FiltersApiModel()
            {
                Desde = DateTime.Now.AddDays(-7);
                Hasta = DateTime.Now;
            }
        }

        public class FacturaTotales
        {
            public decimal SubtotalItemsMasIva { get; set; }
            public decimal SubtotalItemsIva { get; set; }
            public decimal SubtotalItemsICE { get; set; }
            public decimal SubtotalItemsCeroIva { get; set; }
            public decimal SubtotalItemsNoGrabaIva { get; set; }
            public decimal TotalDocumentoElectronico { get; set; }
            public decimal TotalDescuento { get; set; }
        }

        #endregion

        #endregion








        public class MenuItemOption
        {
            public long Id { get; set; }
            public string Name { get; set; }

            public string ImageIcon { get; set; }

        }
    }
}

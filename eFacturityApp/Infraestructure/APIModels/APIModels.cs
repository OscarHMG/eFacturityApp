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

        public class ProductoModel
        {
            public long IdProducto { set; get; }
            public long IdTipoItem { set; get; }
            public long? IdCategoria { set; get; }
            public long? IdUnidadMedida { set; get; }
            public long IdImpuesto { set; get; }

            public string Codigo { set; get; }
            public string Nombre { set; get; }

            public decimal Precio { set; get; }

            public string CodigoAuxiliar { set; get; }
            public string Descripcion { set; get; }
            public string ProductoIva { set; get; }
            public bool? PrecioManual { set; get; }
        }



        public class PersonaModel
        {
            public long IdPersona { set; get; }
            public long IdTipoTipoPersona { set; get; }
            public bool EsContribuyenteEspecial { set; get; }

            public string Identificacion { set; get; }


            public string Ruc { set; get; }
            public string RazonSocial { set; get; }
            public string NombreComercial { set; get; }
            public string Direccion { set; get; }
            public string Telefono { set; get; }
            public bool Extranjero { set; get; }
            public string Correo { set; get; }
            public string Correo2 { set; get; }

            public long? IdUsuario { set; get; }
            public bool? EsAgenteRetencion { set; get; }
            public long? IdRegimen { set; get; }
            public string NumIdentificacion { set; get; }
            public bool? ObligadoLlevarContabilidad { set; get; }
            public string NumeroResolucion { set; get; }
            public long? IdTipoExportador { set; get; }
            public long? IdTipoRegimen { set; get; }
            public string Ciudad { set; get; }
            public string Movil { set; get; }
            public long? IdActividadEconomica { set; get; }
            public long? IdTipoContribuyenteRimpe { set; get; }

            public long IdEmpresa { set; get; }
        }

        #endregion








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

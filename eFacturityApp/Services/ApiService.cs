using eFacturityApp.Infraestructure.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;

namespace eFacturityApp.Services
{
    public class ApiService : APIServiceBase
    {
#if DEBUG
        public const string BASE_URL = "https://714b-181-199-62-229.ngrok.io/";
#else
        public const string BASE_URL = "https://api.efacturity.com:44372/";
#endif
        private const string BASE_API = "api/";
        private const string GET_PROFILE_INFORMATION = BASE_URL + BASE_API + "Perfil/GetPerfil";
        private const string EDIT_PROFILE_INFORMATION = BASE_URL + BASE_API + "Perfil/Editar";
        private const string CHANGE_PASSWORD = BASE_URL + BASE_API + "Seguridad/ResetPassword";
        private const string LOAD_PRODUCTS = BASE_URL + BASE_API + "Catalogos/GetProductos";
        private const string CREATE_EDIT_PRODUCTS = BASE_URL + BASE_API + "Catalogos/CrearEditarProducto";
        private const string DELETE_PRODUCTS = BASE_URL + BASE_API + "Catalogos/EliminarProducto?id={0}";

        private const string LOAD_CLIENTE_PROVEEDORES = BASE_URL + BASE_API + "Catalogos/GetPersonas";

        private const string CREATE_EDIT_CLIENTE_PROVEEDORES = BASE_URL + BASE_API + "Catalogos/CrearEditarPersona";
        private const string DELETE_CLIENTE_PROVEEDOR = BASE_URL + BASE_API + "Catalogos/EliminarPersona?id={0}";

        private const string CATALOGOS_PRODUCTOS_SERVICIOS = BASE_URL + BASE_API + "Catalogos/GetCatalogosProducto";
        //FACTURA
        private const string CATALOGOS_FACTURA = BASE_URL + BASE_API + "Facturas/GetCatalogos";
        private const string LOAD_PUNTOS_VENTA_POR_ESTABLECIMIENTO = BASE_URL + BASE_API + "Facturas/GetPuntosVenta?id={0}";
        private const string CREATE_NEW_FACTURA = BASE_URL + BASE_API + "Facturas/Registrar";
        private const string LOAD_FACTURAS_CREADAS = BASE_URL + BASE_API + "Facturas/GetFacturas";
        private const string COBRAR_FACTURA = BASE_URL + BASE_API + "Facturas/Cobrar?id={0}";
        private const string ENVIAR_FACTURA_SRI = BASE_URL + BASE_API + "Facturas/EnviarSRIFactura?id={0}";
        private const string ANULAR_FACTURA = BASE_URL + BASE_API + "Facturas/Anular?id={0}";
        private const string RECOVER_ACCOUNT = BASE_URL + BASE_API + "Seguridad/OlvideMiContrasenia?correoUsuario={0}";
        private const string GET_TOTALES_FACTURA = BASE_URL + BASE_API + "Facturas/CalcularTotales";
        public string DOWNLOAD_DOC { get; set; } = BASE_URL + BASE_API + "Facturas/Descargar?id={0}&extension={1}";
        //PROFORMA
        private const string CREATE_NEW_PROFORMA = BASE_URL + BASE_API + "Proformas/Registrar";
        private const string GET_TOTALES_PROFORMA = BASE_URL + BASE_API + "Proformas/CalcularTotales";
        private const string LOAD_PROFORMAS_CREADAS = BASE_URL + BASE_API + "Proformas/GetProformas";

        //LIQ DE COMPRA
        private const string GET_TOTALES_LIQ_COMPRA = BASE_URL + BASE_API + "Liquidaciones/CalcularTotales";
        private const string CREATE_LIQUIDACION_COMPRA = BASE_URL + BASE_API + "Liquidaciones/Registrar";
        private const string LOAD_LIQUIDACION_COMPRA_CREADAS = BASE_URL + BASE_API + "Liquidaciones/GetLiquidaciones";
        private const string COBRAR_LIQUIDACION_COMPRA = BASE_URL + BASE_API + "Liquidaciones/Cobrar?id={0}";
        private const string ENVIAR_LIQUIDACION_COMPRA_SRI = BASE_URL + BASE_API + "Liquidaciones/EnviarSRI?id={0}";
        private const string ANULAR_LIQUIDACION_COMPRA = BASE_URL + BASE_API + "Liquidaciones/Anular?id={0}";
        
        public string DOWNLOAD_DOC_LIQ_COMPRA { get; set; } = BASE_URL + BASE_API + "Liquidaciones/Descargar?id={0}&extension={1}";
        //NOTA DE DEBITO
        private const string CREATE_NEW_NOTA_DEBITO = BASE_URL + BASE_API + "NotasDebito/Registrar";
        private const string GET_NOTAS_DEBITO = BASE_URL + BASE_API + "NotasDebito/GetNotasDebito";
        private const string ANULAR_NOTA_DEBITO = BASE_URL + BASE_API + "NotasDebito/Anular?id={0}";
        private const string ENVIAR_NOTA_DEBITO_SRI = BASE_URL + BASE_API + "NotasDebito/EnviarSRI?id={0}";
        private const string COBRAR_NOTA_DEBITO = BASE_URL + BASE_API + "NotasDebito/Cobrar?id={0}";
        public string DOWNLOAD_DOC_NOTA_DEBITO { get; set; } = BASE_URL + BASE_API + "NotasDebito/Descargar?id={0}&extension={1}";


        //NOTA DE CREDITO
        private const string GET_NOTAS_CREDITO_FACTURA = BASE_URL + BASE_API + "NotasCredito/GetNotasCreditoFactura?idEmpresa={0}&desde={1}&hasta={2}";
        private const string GET_NOTAS_CREDITO_LIQ_COMPRA = BASE_URL + BASE_API + "NotasCredito/GetNotasCreditoLiquidacion?idEmpresa={0}&desde={1}&hasta={2}";
        private const string GET_DETALLE_FACTURA = BASE_URL + BASE_API + "NotasCredito/GetDatosFacturaRelacionada?idDocumentoCabecera={0}";
        private const string GET_DETALLE_LIQ_COMPRAS = BASE_URL + BASE_API + "NotasCredito/GetDatosLiquidacionRelacionada?idDocumentoCabecera={0}";
        private const string CREATE_NEW_NOTA_CREDITO = BASE_URL + BASE_API + "NotasCredito/Registrar";
        private const string LOAD_NOTA_CREDITOS_CREADAS = BASE_URL + BASE_API + "Facturas/GetFacturas";


        private const string ENVIAR_CORREO = BASE_URL + BASE_API + "DcumentoEmail/EnviarDocumento";
        


        public ApiService() : base()
        {
            Client.BaseAddress = new Uri(BASE_URL);
        }


        public async Task<GenericResponseObject<PerfilUsuarioModel>> GetProfileInformation(LoginUsuarioRequest credentials)
        {
            return await this.GetAsync<GenericResponseObject<PerfilUsuarioModel>>(GET_PROFILE_INFORMATION, null);
        }


        public async Task<GenericResponseObject<object>> ChangePassword(ChangePasswordAPIModel data)
        {
            return await this.PostAsync<ChangePasswordAPIModel, GenericResponseObject<object>>(data, CHANGE_PASSWORD, null);
        }


        public async Task<GenericResponseObject<CatalogosProductoModel>> GetCatalogosProducto()
        {
            return await this.GetAsync<GenericResponseObject<CatalogosProductoModel>>(CATALOGOS_PRODUCTOS_SERVICIOS, null);
        }

        public async Task<GenericResponseObject<List<ProductoModel>>> LoadProducts()
        {
            return await this.GetAsync<GenericResponseObject<List<ProductoModel>>>(LOAD_PRODUCTS, null);
        }

        

        public async Task<GenericResponseObject<object>> CreateEditProduct(ProductoModel data)
        {
            return await this.PostAsync<ProductoModel, GenericResponseObject<object>>(data, CREATE_EDIT_PRODUCTS, null);
        }


        public async Task<GenericResponseObject<object>> DeleteProduct(long Id)
        {
            return await this.GetAsync<GenericResponseObject<object>>(DELETE_PRODUCTS, new string[] { Id.ToString() });
        }


        public async Task<GenericResponseObject<List<PersonaModel>>> LoadClienteProveedores()
        {
            return await this.GetAsync<GenericResponseObject<List<PersonaModel>>>(LOAD_CLIENTE_PROVEEDORES, null);
        }

        public async Task<GenericResponseObject<object>> DeleteClienteProveedor(long idPersona)
        {
            return await this.GetAsync<GenericResponseObject<object>>(DELETE_CLIENTE_PROVEEDOR, new string[] { idPersona.ToString() });
        }

        public async Task<GenericResponseObject<object>> CreateEditClienteProveedor(PersonaModel data)
        {
            return await this.PostAsync<PersonaModel, GenericResponseObject<object>>(data, CREATE_EDIT_CLIENTE_PROVEEDORES, null);
        }


        public async Task<GenericResponseObject<object>> UpdateProfileInformation(PerfilUsuarioModel data)
        {
            return await this.PostAsync<PerfilUsuarioModel, GenericResponseObject<object>>(data, EDIT_PROFILE_INFORMATION, null);
        }

        public async Task<GenericResponseObject<CatalogosApiModel>> GetCatalogosFactura()
        {
            return await this.GetAsync<GenericResponseObject<CatalogosApiModel>>(CATALOGOS_FACTURA);
        }


        public async Task<GenericResponseObject<List<PuntoVentaModel>>> GetCatalogosPuntoVenta(long IdEstablecimiento)
        {
            return await this.GetAsync<GenericResponseObject<List<PuntoVentaModel>>>(LOAD_PUNTOS_VENTA_POR_ESTABLECIMIENTO, new object[] { IdEstablecimiento.ToString()});
        }

        public async Task<GenericResponseObject<FacturaModel>> CreateNewFactura(FacturaModel data)
        {
            return await this.PostAsync<FacturaModel, GenericResponseObject<FacturaModel>>(data, CREATE_NEW_FACTURA, null);
        }

        public async Task<GenericResponseObject<ListarDocumentosGeneradosModel>> GetConsultaFacturas(FiltersApiModel filtros)
        {
            return await this.PostAsync<FiltersApiModel, GenericResponseObject<ListarDocumentosGeneradosModel>>(filtros, LOAD_FACTURAS_CREADAS);
        }


        public async Task<GenericResponseObject<object>> CobrarFactura(long Id)
        {
            return await this.GetAsync<GenericResponseObject<object>>(COBRAR_FACTURA, new object[] { Id.ToString() });
        }


        public async Task<GenericResponseObject<object>> EnviarSRIFactura(long Id)
        {
            return await this.PostAsync<GenericResponseObject<object>>(ENVIAR_FACTURA_SRI, null, new object[] { Id.ToString() });
        }

        public async Task<GenericResponseObject<object>> AnularFactura(long Id)
        {
            return await this.PostAsync<GenericResponseObject<object>>(ANULAR_FACTURA, null, new object[] { Id.ToString() });
        }

        public async Task<GenericResponseObject<object>> RecoverAccount(string Correo)
        {
            return await this.PostAsync<GenericResponseObject<object>>(RECOVER_ACCOUNT, null, new object[] { Correo.ToString() });
        }

        public async Task<GenericResponseObject<FacturaTotales>> CalcularTotales(List<ItemFacturaModel> items)
        {
            return await this.PostAsync<List<ItemFacturaModel>, GenericResponseObject<FacturaTotales>>(items, GET_TOTALES_FACTURA, null);
        }

        //////////// PROFORMA ////////////
        public async Task<GenericResponseObject<ProformaModel>> CreateNewProforma(ProformaModel data)
        {
            return await this.PostAsync<ProformaModel, GenericResponseObject<ProformaModel>>(data, CREATE_NEW_PROFORMA, null);
        }
        public async Task<GenericResponseObject<ListarProformaGeneradasViewModel>> GetConsultaProformas(FiltersApiModel filtros)
        {
            return await this.PostAsync<FiltersApiModel, GenericResponseObject<ListarProformaGeneradasViewModel>>(filtros, LOAD_PROFORMAS_CREADAS);
        }


        public async Task<GenericResponseObject<ProformaTotales>> CalcularTotalesProforma(List<ItemProforma> items)
        {
            return await this.PostAsync<List<ItemProforma>, GenericResponseObject<ProformaTotales>>(items, GET_TOTALES_PROFORMA, null);
        }

        //////////   LIQUIDACION DE COMPRA  //////////////

        public async Task<GenericResponseObject<LiquidacionCompraTotales>> CalcularTotalesLiquidacionCompra(List<ItemLiquidacionCompraModel> items)
        {
            return await this.PostAsync<List<ItemLiquidacionCompraModel>, GenericResponseObject<LiquidacionCompraTotales>>(items, GET_TOTALES_LIQ_COMPRA, null);

        }

        public async Task<GenericResponseObject<LiquidacionCompraModel>> CreateNewLiquidacionCompra(LiquidacionCompraModel data)
        {
            return await this.PostAsync<LiquidacionCompraModel, GenericResponseObject<LiquidacionCompraModel>>(data, CREATE_LIQUIDACION_COMPRA, null);
        }

        public async Task<GenericResponseObject<ListarLiquidacionCompraModel>> GetConsultaLiquidacionCompra(FiltersApiModel filtros)
        {
            return await this.PostAsync<FiltersApiModel, GenericResponseObject<ListarLiquidacionCompraModel>>(filtros, LOAD_LIQUIDACION_COMPRA_CREADAS);
        }

        public async Task<GenericResponseObject<object>> CobrarLiquidacionCompra(long Id)
        {
            return await this.GetAsync<GenericResponseObject<object>>(COBRAR_LIQUIDACION_COMPRA, new object[] { Id.ToString() });
        }


        public async Task<GenericResponseObject<object>> EnviarSRILiquidacionCompra(long Id)
        {
            return await this.PostAsync<GenericResponseObject<object>>(ENVIAR_LIQUIDACION_COMPRA_SRI, null, new object[] { Id.ToString() });
        }

        public async Task<GenericResponseObject<object>> AnularLiquidacionCompra(long Id)
        {
            return await this.PostAsync<GenericResponseObject<object>>(ANULAR_LIQUIDACION_COMPRA, null, new object[] { Id.ToString() });
        }


        public async Task<GenericResponseObject<object>> EnviarCorreo(EnviarDocumentoDocumentoModel data)
        {
            return await this.PostAsync<EnviarDocumentoDocumentoModel, GenericResponseObject<object>>(data, ENVIAR_CORREO, null);
        }


        //NOTA DE DEBITO
        public async Task<GenericResponseObject<NotaDebitoModel>> CreateNewNotaDebito(NotaDebitoModel data)
        {
            return await this.PostAsync<NotaDebitoModel, GenericResponseObject<NotaDebitoModel>>(data, CREATE_NEW_NOTA_DEBITO, null);
        }

        public async Task<GenericResponseObject<object>> EnviarNotaDebitoSRI(long Id)
        {
            return await this.PostAsync<GenericResponseObject<object>>(ENVIAR_NOTA_DEBITO_SRI, null, new object[] { Id.ToString() });
        }

        public async Task<GenericResponseObject<ListarNotasDebitoModel>> GetConsultaNotasDebito(FiltersApiModel filtros)
        {
            return await this.PostAsync<FiltersApiModel, GenericResponseObject<ListarNotasDebitoModel>>(filtros, GET_NOTAS_DEBITO);
        }

        public async Task<GenericResponseObject<object>> AnularNotaDebito(long Id)
        {
            return await this.PostAsync<GenericResponseObject<object>>(ANULAR_NOTA_DEBITO, null, new object[] { Id.ToString() });
        }

        public async Task<GenericResponseObject<object>> CobrarNotaDebito(long Id)
        {
            return await this.GetAsync<GenericResponseObject<object>>(COBRAR_NOTA_DEBITO, new object[] { Id.ToString() });
        }

        //NOTA DE CREDITO
        public async Task<GenericResponseObject<List<DocumentosRelacionados>>> GetFacturasNotaCredito(long Id, DateTime desde, DateTime hasta)
        {
            return await this.PostAsync<object, GenericResponseObject<List<DocumentosRelacionados>>>(null, GET_NOTAS_CREDITO_FACTURA, new object[] { Id.ToString(), desde.ToString("yyyy-MM-dd"), hasta.ToString("yyyy-MM-dd") });
        }

        public async Task<GenericResponseObject<List<DocumentosRelacionados>>> GetLiqCompraNotaCredito(long Id, DateTime desde, DateTime hasta)
        {
            return await this.PostAsync<object, GenericResponseObject<List<DocumentosRelacionados>>>(null, GET_NOTAS_CREDITO_LIQ_COMPRA, new object[] { Id.ToString(), desde.ToString("yyyy-MM-dd"), hasta.ToString("yyyy-MM-dd") });
        }


        public async Task<GenericResponseObject<NotaCreditoModel>> GetDetalleNotaCreditoFactura(long Id)
        {
            return await this.GetAsync<GenericResponseObject<NotaCreditoModel>>(GET_DETALLE_FACTURA, new object[] { Id.ToString() });
        }

        public async Task<GenericResponseObject<NotaCreditoModel>> GetDetalleNotaCreditoliqCompra(long Id)
        {
            return await this.GetAsync<GenericResponseObject<NotaCreditoModel>>(GET_DETALLE_LIQ_COMPRAS, new object[] { Id.ToString() });
        }

        public async Task<GenericResponseObject<NotaCreditoModel>> CreateNewNotaCredito(NotaCreditoModel data)
        {
            return await this.PostAsync<NotaCreditoModel, GenericResponseObject<NotaCreditoModel>>(data, CREATE_NEW_NOTA_CREDITO, null);
        }

        public async Task<GenericResponseObject<ListarDocumentosNotaCreditoGeneradosViewModel>> GetConsultaNotaCreditos(FiltersApiModel filtros)
        {
            return await this.PostAsync<FiltersApiModel, GenericResponseObject<ListarDocumentosNotaCreditoGeneradosViewModel>>(filtros, LOAD_NOTA_CREDITOS_CREADAS);
        }
    }
}

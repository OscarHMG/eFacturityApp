﻿using eFacturityApp.Infraestructure.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;

namespace eFacturityApp.Services
{
    public class ApiService : APIServiceBase
    {
        public const string BASE_URL = "https://da56-2800-bf0-8027-1187-5080-2157-d690-2951.ngrok.io/";

        private const string BASE_API = "api/";
        private const string GET_PROFILE_INFORMATION = BASE_URL + BASE_API + "Perfil/GetPerfil";
        private const string EDIT_PROFILE_INFORMATION = BASE_URL + BASE_API + "Perfil/Editar";
        private const string CHANGE_PASSWORD = BASE_URL + BASE_API + "Seguridad/ResetPassword";
        private const string LOAD_PRODUCTS = BASE_URL + BASE_API + "Catalogos/GetProductos";
        private const string CREATE_EDIT_PRODUCTS = BASE_URL + BASE_API + "Catalogos/CrearEditarProducto";
        private const string DELETE_PRODUCTS = BASE_URL + BASE_API + "Catalogos/EliminarProducto?id={0}";

        private const string LOAD_CLIENTE_PROVEEDORES = BASE_URL + BASE_API + "Catalogos/GetPersonas";

        private const string CREATE_EDIT_CLIENTE_PROVEEDORES = BASE_URL + BASE_API + "Catalogos/CrearEditarPersona";
        private const string DELETE_CLIENTE_PROVEEDOR =  BASE_URL + BASE_API + "Catalogos/EliminarPersona?id={0}";

        private const string CATALOGOS_PRODUCTOS_SERVICIOS = BASE_URL + BASE_API + "Catalogos/GetCatalogosProducto";

        private const string CATALOGOS_FACTURA = BASE_URL + BASE_API + "Facturas/GetCatalogos";
        private const string LOAD_PUNTOS_VENTA_POR_ESTABLECIMIENTO = BASE_URL + BASE_API + "Facturas/GetPuntosVenta?id={0}";
        private const string CREATE_NEW_FACTURA = BASE_URL + BASE_API + "Facturas/Registrar";
        private const string LOAD_FACTURAS_CREADAS = BASE_URL + BASE_API + "Facturas/GetFacturas";
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
            return await this.GetAsync<FiltersApiModel,GenericResponseObject<ListarDocumentosGeneradosModel>>(filtros, LOAD_FACTURAS_CREADAS);
        }
    }
}

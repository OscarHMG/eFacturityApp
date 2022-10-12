using eFacturityApp.Infraestructure.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;

namespace eFacturityApp.Services
{
    public class ApiService : APIServiceBase
    {
        public const string BASE_URL = "https://49b9-157-100-111-45.ngrok.io/";

        private const string BASE_API = "api/";
        private const string LOGIN = BASE_URL + BASE_API + "Seguridad/Login";
        private const string CHANGE_PASSWORD = BASE_URL + BASE_API + "Seguridad/ResetPassword";
        private const string LOAD_PRODUCTS = BASE_URL + BASE_API + "Catalogos/GetProductos";
        private const string CREATE_EDIT_PRODUCTS = BASE_URL + BASE_API + "Catalogos/CrearEditarProducto";
        private const string DELETE_PRODUCTS = BASE_URL + BASE_API + "Catalogos/EliminarProducto?Id={0}";

        private const string LOAD_CLIENTE_PROVEEDORES = BASE_URL + BASE_API + "Catalogos/GetPersonas";

        private const string CREATE_EDIT_CLIENTE_PROVEEDORES = BASE_URL + BASE_API + "Catalogos/CrearEditarPersona";
        public ApiService() : base()
        {
            Client.BaseAddress = new Uri(BASE_URL);
        }


        public async Task<GenericResponseObject<PerfilUsuarioResponse>> Login(LoginUsuarioRequest credentials)
        {
            return await this.GetAsync<GenericResponseObject<PerfilUsuarioResponse>>(LOGIN, null);
        }


        public async Task<GenericResponseObject<object>> ChangePassword(ChangePasswordAPIModel data)
        {
            return await this.PostAsync<ChangePasswordAPIModel, GenericResponseObject<object>>(data, CHANGE_PASSWORD, null);
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


        public async Task<GenericResponseObject<object>> CreateEditClienteProveedor(PersonaModel data)
        {
            return await this.PostAsync<PersonaModel, GenericResponseObject<object>>(data, CREATE_EDIT_CLIENTE_PROVEEDORES, null);
        }

    }
}

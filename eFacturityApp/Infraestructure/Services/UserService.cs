using eFacturityApp.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace eFacturityApp.Infraestructure.Services
{
    public class UserService
    {
        private ApiService Api { get; set; }
        private INavigationService NavigationService { get; set; }
        public string Username { get; set; }
        public string Patio { get; set; }
        public string RolUsuario { get; set; }
        public UserService(INavigationService navigationService, ApiService api)
        {
            Api = api;
            NavigationService = navigationService;
        }

        public async Task StoreToken(string username, string token, DateTime expiresIn)
        {
            //string expireDate = expiresIn.ToUniversalTime().ToString();
            string expireDate = expiresIn.ToString("yyyy-MM-dd HH:mm:ss");

            Username = username;

            await SecureStorage.SetAsync("username", username);
            await SecureStorage.SetAsync("oauth_token", token);
            await SecureStorage.SetAsync("expiresIn", expireDate);
            // 2022-04-14 Cambio para solucionar bug de validacion de expiracion de token
            await SecureStorage.SetAsync("authVersion", "v1.1");
        }

        public async Task HandleLogin(string Username, string Password)
        {

            var respuesta = await Api.GetToken(Username.Trim(), Password.Trim());


            await StoreToken(Username.Trim(), respuesta.AccessToken, respuesta.Expires);
            var r = await Api.GetUserClient("C");
            if (!r.Equals(Username))
            {
                await StoreToken("", "", DateTime.Now);
            }
        }



        public async Task<string> GetEmail()
        {
            return await SecureStorage.GetAsync("username");
        }

        /// <summary>
        /// Valida la fecha de expiracion del token.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> HasValidToken()
        {
            // 14-04-2022 validacion para corregir error de token
            var authVersion = await SecureStorage.GetAsync("authVersion");
            if (authVersion == null || authVersion != "v1.1")
                return false;

            var token = await SecureStorage.GetAsync("oauth_token");
            if (token == null)
                return false;

            var fechaExpiracionToken = await SecureStorage.GetAsync("expiresIn");
            if (fechaExpiracionToken == null)
                return false;

            try
            {
                var fechaExpiracionReal = DateTime.ParseExact(fechaExpiracionToken, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                return DateTime.Now.CompareTo(fechaExpiracionReal) < 0;
            }
            catch (Exception e)
            {
                // Agregar log para debug.
                return false;
            }

            // Codigo Original
            /*
            var token = await SecureStorage.GetAsync("oauth_token");
            if (token != null)
            {
                var expiresIn = await SecureStorage.GetAsync("expiresIn");
                var expirationDate = Convert.ToDateTime(expiresIn);

                return (DateTime.Now.CompareTo(expirationDate) < 0);
            }
            return false;*/
        }

        public async Task HandleOnResume()
        {
            var hasValidToken = await HasValidToken();
            INavigationResult result;
            if (hasValidToken)
            {
                result = await NavigationService.NavigateAsync("/NavigationPage/DemoPage");
                if (!result.Success)
                {
                    Console.WriteLine(result.Exception.Message);
                }
            }
            else
            {
                CerrarSession();
            }
        }

        public void CerrarSession()
        {
            SecureStorage.RemoveAll();
            //MessagingCenter.Send<String, bool>("UserService", "Session", false);
        }

    }
}

using eFacturityApp.Services;
using Newtonsoft.Json;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static eFacturityApp.Infraestructure.ApiModels.APIModels;

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

        public async Task SaveUserInformationProfile(PerfilUsuarioResponse perfilInformation)
        {
            //string expireDate = perfilInformation.expiresIn.ToString("yyyy-MM-dd HH:mm:ss");

            await SecureStorage.SetAsync("UserInfo", JsonConvert.SerializeObject(perfilInformation));
        }


        public async Task<PerfilUsuarioResponse> GetUserInformationProfile()
        {
            return JsonConvert.DeserializeObject<PerfilUsuarioResponse>(await SecureStorage.GetAsync("UserInfo"));
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
            var token = await SecureStorage.GetAsync("oauth_token");
            if (token != null)
            {
                var expiresIn = await SecureStorage.GetAsync("expiresIn");
                var expirationDate = Convert.ToDateTime(expiresIn);

                return (DateTime.Now.CompareTo(expirationDate) < 0);
            }
            return false;
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

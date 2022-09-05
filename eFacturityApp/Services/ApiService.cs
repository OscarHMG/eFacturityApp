using eFacturityApp.Infraestructure.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eFacturityApp.Services
{
    public class ApiService : APIServiceBase
    {
        public const string BASE_URL = "https://www.foo.com/";

        private const string BASE_API = "api/";


        public ApiService() : base()
        {
            Client.BaseAddress = new Uri(BASE_URL);
        }

        public async Task<string> GetUserClient(string tipo)
        {
            return await this.GetAsync<string>("", new string[] { tipo });
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace eFacturityApp.Infraestructure.ApiModels
{
    public class TokenRequest
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }

        [JsonProperty(".expires")]
        public DateTime Expires { get; set; }
    }
}

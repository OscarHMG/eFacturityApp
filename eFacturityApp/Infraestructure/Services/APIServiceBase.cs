using eFacturityApp.Infraestructure.ApiModels;
using Newtonsoft.Json;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace eFacturityApp.Infraestructure.Services
{
    public class APIServiceBase
    {
#if DEBUG
        
        protected HttpClient Client = PreparedClient();
        public static HttpClient PreparedClient()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(handler); return client;
        }
        
#else

        protected HttpClient Client { get; set; }
        public APIServiceBase()
        {
            Client = new HttpClient() { MaxResponseContentBufferSize = 2560000 };
        }


#endif


        public Task HandleUnauthorized()
        {
            throw new UnauthorizedAccessException("Su sesión es inválida o ha expirado");
        }

        public async Task<TokenRequest> GetToken(string username, string password)
        {
            string endpoint = "Token";

            var dict = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "Username", username },
                { "Password", password }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = new FormUrlEncodedContent(dict) };
            var response = await Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var parsedResponse = JsonConvert.DeserializeObject<TokenRequest>(json);
                return parsedResponse;
            }
            else
            {
                return null;
            }
        }


        public async Task<T> PostAsync<T>(string Endpoint, T data, object[] args = null)
        {
            try
            {
                var oauthToken = await SecureStorage.GetAsync("oauth_token");
                if (!string.IsNullOrEmpty(oauthToken))
                {
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
                }

                Uri uri = null;
                if (args == null || args.Length == 0)
                {
                    uri = new Uri(Endpoint);
                }
                else
                {
                    string url = string.Format(Endpoint, args);
                    uri = new Uri(url);
                }


                var body = JsonConvert.SerializeObject(data);
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage
                {
                    RequestUri = uri,
                    Content = content,
                    Method = HttpMethod.Post,
                    Headers =
                    {
                        {"X-Version", $"{VersionTracking.CurrentVersion} ({VersionTracking.CurrentBuild})" },
                    }
                };
                var response = await Client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var parsedResponse = JsonConvert.DeserializeObject<T>(json);
                    return parsedResponse;
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await HandleUnauthorized();
                        return default;
                    }
                    else
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var parserResponse = JsonConvert.DeserializeObject<BadRequest>(json);
                        throw new Exception(parserResponse.Message);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error al realizar la consulta: \n\n{0}", e.Message), e); ;
            }
        }


        public async Task<U> PostAsync<T, U>(T data, string Endpoint, object[] args = null)
        {
            try
            {
                var oauthToken = await SecureStorage.GetAsync("oauth_token");
                if (!string.IsNullOrEmpty(oauthToken))
                {
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
                }
                Uri uri = null;
                if (args == null || args.Length == 0)
                {
                    uri = new Uri(Endpoint);
                }
                else
                {
                    string url = string.Format(Endpoint, args);
                    uri = new Uri(url);
                }


                var body = JsonConvert.SerializeObject(data);
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage
                {
                    RequestUri = uri,
                    Content = content,
                    Method = HttpMethod.Post,
                    Headers =
                    {
                        {"X-Version", $"{VersionTracking.CurrentVersion} ({VersionTracking.CurrentBuild})" },
                    }
                };
                var response = await Client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var parsedResponse = JsonConvert.DeserializeObject<U>(json);
                    return parsedResponse;
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await HandleUnauthorized();
                        return default;
                    }
                    else
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var parserResponse = JsonConvert.DeserializeObject<BadRequest>(json);
                        throw new Exception(parserResponse.Message);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error al realizar la consulta: \n\n{0}", e.Message), e); ;
            }
        }


        public async Task<T> GetAsync<T>(string Endpoint, object[] args = null)
        {
            var oauthToken = await SecureStorage.GetAsync("oauth_token");
            if (!string.IsNullOrEmpty(oauthToken))
            {
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
            }

            Uri uri = null;
            if (args == null || args.Length == 0)
            {
                uri = new Uri(Endpoint);
            }
            else
            {
                string url = string.Format(Endpoint, args);
                uri = new Uri(url);
            }

            //string url = string.Format(Endpoint, args);
            //var uri = new Uri(url);

            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await Client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var parserResponse = JsonConvert.DeserializeObject<T>(json);
                return parserResponse;
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await HandleUnauthorized();
                    return default;
                }
                else
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var parserResponse = JsonConvert.DeserializeObject<BadRequest>(json);
                    throw new Exception(parserResponse.Message);
                }
            }
        }

        public async Task<U> GetAsync<T, U>(T data, string Endpoint, object[] args = null)
        {
            try
            {
                var oauthToken = await SecureStorage.GetAsync("oauth_token");
                if (!string.IsNullOrEmpty(oauthToken))
                {
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
                }
                Uri uri = null;
                if (args == null || args.Length == 0)
                {
                    uri = new Uri(Endpoint);
                }
                else
                {
                    string url = string.Format(Endpoint, args);
                    uri = new Uri(url);
                }


                var body = JsonConvert.SerializeObject(data);
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage
                {
                    RequestUri = uri,
                    Content = content,
                    Method = HttpMethod.Get,
                    Headers =
                    {
                        {"X-Version", $"{VersionTracking.CurrentVersion} ({VersionTracking.CurrentBuild})" },
                    }
                };
                var response = await Client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var parsedResponse = JsonConvert.DeserializeObject<U>(json);
                    return parsedResponse;
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await HandleUnauthorized();
                        return default;
                    }
                    else
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var parserResponse = JsonConvert.DeserializeObject<BadRequest>(json);
                        throw new Exception(parserResponse.Message);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error al realizar la consulta: \n\n{0}", e.Message), e); ;
            }
        }

        public async Task<bool> PostDataWithPhotos<T>(string EndPoint, T request, List<string> FotosRuta)
        {

            bool enviaImagenes = false;
            List<StreamContent> streamContents = new List<StreamContent>();
            try
            {
                var oauthToken = await SecureStorage.GetAsync("oauth_token");
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);

                //Creo el Multipart.
                var multipartContent = new MultipartFormDataContent();
                multipartContent.Headers.ContentType.MediaType = "multipart/form-data";

                //Agrego el Data.
                multipartContent.Add(new StringContent(JsonConvert.SerializeObject(request)), "Data");

                //Agrego ahora en el multi part, las fotografias.
                string imagetag = "Fotos";
                FotosRuta.ForEach(c =>
                {
                    var fs = new FileStream(c, FileMode.Open);
                    var fn = Path.GetFileName(fs.Name);
                    string filename = fn;
                    var fotoContent = new StreamContent(fs, (int)fs.Length);
                    fotoContent.Headers.TryAddWithoutValidation("Content-Disposition", "form-data; name=" + imagetag + "; filename=" + filename);
                    fotoContent.Headers.TryAddWithoutValidation("Content-Type", "image / png");
                    multipartContent.Add(fotoContent, imagetag, filename);
                    streamContents.Add(fotoContent);
                });


                var uri = new Uri(EndPoint);

                var response = await Client.PostAsync(uri, multipartContent);
                enviaImagenes = response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {

                streamContents.ForEach(x => x.Dispose());
            }

            // Retorna el estado de la llamada
            return enviaImagenes;
        }

    }
}

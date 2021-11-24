using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Passwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Passwork
{

    class Connection : IConnection
    {
        private AuthLoginResponse loginResponse;
        private string baseUrl;
        private string apikey;
        private string token;
        private string lastError;
        private string masterHash;
        private string masterPassword;


        private static HttpClient httpClient = new HttpClient();

        public string UserName => loginResponse.user.name;
        public string UserMail => loginResponse.user.email;
        public string LastError => lastError;
        public bool WithMasterPassword => !string.IsNullOrEmpty(masterHash);
        public string MasterPassword => masterPassword;

        public Connection(string baseUrl, AuthLoginResponse response, string apikey, string masterPassword="")
        {
            this.loginResponse = response;
            this.token = response.token;
            this.baseUrl = baseUrl;
            this.apikey = apikey;
            this.masterPassword = masterPassword;
            this.masterHash = string.IsNullOrEmpty(masterPassword)? "" : CryptoUtils.Hash(masterPassword);
        }

 
        public async Task<TResponse> Get<TResponse>(string url)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{baseUrl}{url}"),
                Headers =
                {
                    { "Passwork-Auth", token },
                    { "Passwork-MasterHash", masterHash }
                },
            };

            var result = await httpClient.SendAsync(request);
            var content = await result.Content.ReadAsStringAsync();
            var x = JsonConvert.DeserializeObject<Response<TResponse>>(content);
            if (x.status == "success")
            {
                return x.data;
            }
            lastError = $"{x.status} - {x.code} - {x.data}";
            return default(TResponse);
        }

        public async Task<Response<TResponse>> Post<T, TResponse>(string url, T payload)
        {
            string payloadString = "";
            if (payload != null) { payloadString = JsonConvert.SerializeObject(payload); }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{baseUrl}{url}"),
                Headers =
                {
                    { "Passwork-Auth", token },
                    { "Passwork-MasterHash", masterHash }
                },
                Content = new StringContent(payloadString)
            };

            var result = await httpClient.SendAsync(request);
            var content = await result.Content.ReadAsStringAsync();

            //first, parse the response as object... in case of an error, the .data part of the response 
            //might not be TResponse..
            var value = JsonConvert.DeserializeObject<Response<object>>(content);

            //var value = JsonConvert.DeserializeObject<Response<TResponse>>(content);
            if (value.status != "success")
            {
                lastError = $"{value.status} - {value.code} - {value.data}";
                return new Response<TResponse>
                {
                    code = value.code,
                    status = value.status,
                    data = default(TResponse)
                };
            }
            else
            {
                //value
                return new Response<TResponse>
                {
                    code = value.code,
                    status = value.status,
                    data = typeof(TResponse).IsArray?((JArray)value.data).ToObject<TResponse>() : ((JObject)value.data).ToObject<TResponse>()
                };
            }
        }

            public async Task<Response<TResponse>> Put<T, TResponse>(string url, T payload)
        {
            string payloadString = "";
            if (payload != null) { payloadString = JsonConvert.SerializeObject(payload); }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{baseUrl}{url}"),
                Headers =
                {
                    { "Passwork-Auth", token },
                    { "Passwork-MasterHash", masterHash }
                },
                Content = new StringContent(payloadString)
            };

            var result = await httpClient.SendAsync(request);
            var content = await result.Content.ReadAsStringAsync();
            var value = JsonConvert.DeserializeObject<Response<TResponse>>(content);
            if (value.status != "success")
            {
                lastError = $"{value.status} - {value.code} - {value.data}";
            }
            return value;
        }

        public async Task<Response<TResponse>> Delete<TResponse>(string url)
        {
            
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{baseUrl}{url}"),
                Headers =
                {
                    { "Passwork-Auth", token },
                    { "Passwork-MasterHash", masterHash }
                }
            };

            var result = await httpClient.SendAsync(request);
            var content = await result.Content.ReadAsStringAsync();
            var value = JsonConvert.DeserializeObject<Response<TResponse>>(content);
            if (value.status != "success")
            {
                lastError = $"{value.status} - {value.code} - {value.data}";
            }
            return value;
        }


    }
}

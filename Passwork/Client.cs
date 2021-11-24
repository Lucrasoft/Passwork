using Newtonsoft.Json;
using Passwork.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Passwork
{
    public class Client
    {

        private readonly string baseUrl;
        private string token;

        private static HttpClient httpClient = new HttpClient();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUrl">Should be in the format https://your-server/api/v4  </param>
        public Client(string baseUrl)
        {
            this.baseUrl = baseUrl;
            if (!this.baseUrl.EndsWith('/')) { this.baseUrl += "/"; }
        }


        /// <summary>
        /// Login at the provided baseUrl to get an IContext by 
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="masterPass">The masterpassword (optional)</param>
        /// <returns>IContext if successfull, otherwise null</returns>
        public async Task<IContext> LoginAsync(string apiKey, string masterPass = "")
        {

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{baseUrl}auth/login/{apiKey}")
            };

            var response = await httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Response<AuthLoginResponse>>(content);

            if (result.status == "success")
            {
                this.token = result.data.token;

                var conn = new Connection(baseUrl, result.data, apiKey,masterPass);
                //before return the context, check the working of the masterPass by requesting a domain object.

                var domain = await conn.Get<Domain>("vaults/domain");
                if (domain==null) { return null; }

                return new Context(conn);
            }
            return null;
        }



    }
}

using BarclaysInterBankApp.Infastructure.HttpHelper.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection.Metadata;

namespace BarclaysInterBankApp.Infastructure.HttpHelper.Implementation
{
    public class HttpService : IHttpService
    {
        private readonly IHttpClientFactory _clientFactory;

        public HttpService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<ApiResponse<T>> MakeHttpRequestAsync<T>(Uri requestUri, string payload, string authToken, AuthType authType, CustomHttpMethod httpMethod)
        {
            var client = _clientFactory.CreateClient();

            if (authType == AuthType.Basic)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            }
            else if (authType == AuthType.Bearer)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }
            else
            {
                client.DefaultRequestHeaders.Authorization = null;
            }

            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Method = new HttpMethod(httpMethod.ToString()),
                RequestUri = requestUri,
            };

            if(!string.IsNullOrEmpty(payload))
            {
                StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                requestMessage.Content = content;
                Console.WriteLine("Content-Type: " + content.Headers.ContentType);
            }

            HttpResponseMessage responseMessage = new HttpResponseMessage
            {
                ReasonPhrase = "Network Error",
                StatusCode = System.Net.HttpStatusCode.GatewayTimeout
            };
            string responseString = string.Empty;
            try
            {
                responseMessage = await client.SendAsync(requestMessage);
                responseString = await responseMessage.Content.ReadAsStringAsync();

                string payloadFromContent = await responseMessage.Content.ReadAsStringAsync();
                Console.WriteLine("Payload from content: " + payloadFromContent);

                responseMessage.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                responseMessage.ReasonPhrase += $"{ex.Message}";
            }
            var response = new ApiResponse<T>
            {
                IsSuccess = responseMessage.IsSuccessStatusCode,
                StatusCode = (int)responseMessage.StatusCode,
                Data = JsonConvert.DeserializeObject<T>(responseString)
            };
            requestMessage.Dispose();
            requestMessage.Dispose();
            return response;
        }
    }
}

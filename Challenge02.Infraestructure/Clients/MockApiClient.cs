using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using HttpClientService.Interfaces;
using HttpClientService.Models;
using Challenge02.Infraestructure.Clients.Abstractions;
using Challenge02.Infraestructure.Clients.Contracts;
using System.Text;

namespace Challenge02.Infraestructure.Clients
{
    public class MockApiClient : IMockApiClient
    {
        private readonly string _baseUrl;
        private readonly IHttpClientWrapper _httpClient;
        private readonly JsonSerializerSettings _serializerSettings;

        public MockApiClient(
            string baseUrl,
            IHttpClientWrapper httpClient)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
        }

        public Task<HttpResponse<TResponse>> SendAsync<TResponse>(MockApiRequest request) where TResponse : class
        {
            var (endpoint, httpMethod) = request switch
            {
                CreateDevsRequest => ($"{_baseUrl}/DevTest/Dev", HttpMethod.Post),
                UpdateDevsRequest r => ($"{_baseUrl}/DevTest/Dev/{r.Id}", HttpMethod.Put),
                GetDevsRequest r => (r.Id is null
                    ? $"{_baseUrl}/DevTest/Dev"
                    : $"{_baseUrl}/DevTest/Dev/{r.Id}", HttpMethod.Get),
                _ => throw new NotImplementedException($"Request of type '{request.GetType().Name}' not supported for mock client")
            };

            var httpMessage = new HttpRequestMessage(httpMethod, endpoint);

            if (httpMethod != HttpMethod.Get)
            {
                var payload = JsonConvert.SerializeObject(request, _serializerSettings);
                httpMessage.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            }

            var response = _httpClient.SendAsync<TResponse>(httpMessage);

            return response;
        }
    }
}

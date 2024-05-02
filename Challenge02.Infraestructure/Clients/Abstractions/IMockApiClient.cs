using HttpClientService.Models;
using Challenge02.Infraestructure.Clients.Contracts;

namespace Challenge02.Infraestructure.Clients.Abstractions
{
    public interface IMockApiClient
    {
        Task<HttpResponse<TResponse>> SendAsync<TResponse>(MockApiRequest request) where TResponse : class;
    }
}

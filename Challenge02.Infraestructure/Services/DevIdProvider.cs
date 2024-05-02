using Microsoft.EntityFrameworkCore;
using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;
using Challenge02.Infraestructure.Clients.Abstractions;
using Challenge02.Infraestructure.Clients.Contracts;
using Challenge02.Infraestructure.Repository;
using ILogger = Serilog.ILogger;

namespace Challenge02.Infraestructure.Services
{
    public class DevIdProvider : IDevIdProvider
    {
        private readonly IMockApiClient _mockApiClient;
        private readonly DesafioContext _desafioContext;
        private readonly ILogger _logger;

        public DevIdProvider(
            IMockApiClient mockApiClient,
            DesafioContext desafioContext,
            ILogger logger)
        {
            _mockApiClient = mockApiClient ?? throw new ArgumentNullException(nameof(mockApiClient));
            _desafioContext = desafioContext ?? throw new ArgumentNullException(nameof(desafioContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> GetNextIdAsync()
        {
            try
            {
                var available = false;
                var nextId = await _desafioContext.Devs.MaxAsync(x => x.Id) + 1;

                while (available == false)
                {
                    var result = await _mockApiClient.SendAsync<Dev>(new GetDevsRequest(nextId));

                    available = !result.Success;
                }

                return nextId;
            }
            catch (HttpRequestException e)
            {
                throw new Exception("Erro getting next id", e);
            }
        }
    }
}

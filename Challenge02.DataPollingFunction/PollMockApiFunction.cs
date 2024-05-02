using MediatR;
using Microsoft.Azure.WebJobs;
using Challenge02.Domain.Models;
using Challenge02.Infraestructure.Clients.Abstractions;
using Challenge02.Infraestructure.Clients.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ILogger = Serilog.ILogger;

namespace Challenge02.DataPollingFunction
{
    public class PollMockApiFunction
    {
        private readonly IDataSyncService _dataSyncService;
        private readonly IMockApiClient _mockApiClient;
        private readonly ILogger _logger;

        public PollMockApiFunction(
            IMockApiClient mockApiClient,
            IDataSyncService dataSyncService,
            ILogger logger)
        {
            _dataSyncService = dataSyncService ?? throw new ArgumentNullException(nameof(dataSyncService));
            _mockApiClient = mockApiClient ?? throw new ArgumentNullException(nameof(mockApiClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName("PollMockApiFunction")]
        public async Task Run(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            _logger.Information($"PollMockApiFunction executed at: {DateTime.Now}");

            var result = await _mockApiClient.SendAsync<IEnumerable<Dev>>(new GetDevsRequest());

            if (!result.Success)
            {
                _logger.Error("Error getting devs from mock api", result.Errors);
                throw new AggregateException(result.Errors);
            }

            var devsFromSource = result.Response;

            var tasks = new List<Task>
            {
                _dataSyncService.SyncMongoDatabase(devsFromSource),
                _dataSyncService.SyncSqlDatabase(devsFromSource)
            };

            await Task.WhenAll(tasks);
        }
    }
}

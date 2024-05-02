using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Challenge02.Domain.Models;
using Challenge02.Infraestructure.Clients.Abstractions;
using Challenge02.Infraestructure.Clients.Contracts;
using Challenge02.Infraestructure.Repository;
using ILogger = Serilog.ILogger;

public class PollingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMockApiClient _mockApiClient;
    private readonly ILogger _logger;
    private readonly int _pollingInterval;

    public PollingService(
            IServiceProvider serviceProvider,
            IMockApiClient mockApiClient,
            ILogger logger,
            int pollingInterval)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _mockApiClient = mockApiClient ?? throw new ArgumentNullException(nameof(mockApiClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pollingInterval = pollingInterval;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await PollExternalDataSourceAsync();
            await Task.Delay(TimeSpan.FromSeconds(_pollingInterval), cancellationToken);
        }
    }

    private async Task PollExternalDataSourceAsync()
    {
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var result = await _mockApiClient.SendAsync<IEnumerable<Dev>>(new GetDevsRequest());

                if (!result.Success)
                {
                    _logger.Error("Error getting devs from mock api", result.Errors);
                    throw new AggregateException(result.Errors);
                }

                var devsFromSource = result.Response;

                await SyncSqlDatabase(scope, devsFromSource);

                await SyncMongoDatabase(scope, devsFromSource);

                _logger.Information("Sync completed successfully, all data is up to date.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error polling external data source");
        }
    }

    private async Task SyncMongoDatabase(IServiceScope scope, IEnumerable<Dev> devsFromSource)
    {
        var devsCollection = scope.ServiceProvider.GetRequiredService<IMongoCollection<Dev>>();
        var existingDevs = await devsCollection
                            .Find(_ => true)
                            .ToListAsync();

        var updates = new List<WriteModel<Dev>>();

        foreach (var dev in devsFromSource)
        {
            var existingDev = existingDevs.FirstOrDefault(d => d.Id == dev.Id);
            if (existingDev != null)
            {
                bool needsUpdate = existingDev.Avatar != dev.Avatar ||
                                   existingDev.Email != dev.Email ||
                                   existingDev.Squad != dev.Squad ||
                                   existingDev.Login != dev.Login ||
                                   existingDev.Name != dev.Name;

                if (needsUpdate)
                {
                    var filter = Builders<Dev>.Filter.Eq(d => d.Id, dev.Id);
                    var update = Builders<Dev>.Update
                        .Set(d => d.Avatar, dev.Avatar)
                        .Set(d => d.Email, dev.Email)
                        .Set(d => d.Squad, dev.Squad)
                        .Set(d => d.Login, dev.Login)
                        .Set(d => d.Name, dev.Name);

                    updates.Add(new UpdateOneModel<Dev>(filter, update) { IsUpsert = true });
                }
            }
            else
            {
                updates.Add(new InsertOneModel<Dev>(dev));
            }
        }

        var devsToRemove = existingDevs.Where(d => !devsFromSource.Any(ds => ds.Id == d.Id));
        foreach (var dev in devsToRemove)
        {
            var filter = Builders<Dev>.Filter.Eq(d => d.Id, dev.Id);
            updates.Add(new DeleteOneModel<Dev>(filter));
        }

        if (updates.Any())
        {
            const int batchSize = 100;
            for (int i = 0; i < updates.Count; i += batchSize)
            {
                var batch = updates.Skip(i).Take(batchSize).ToList();
                await devsCollection.BulkWriteAsync(batch);
            }
        }
    }

    private async Task SyncSqlDatabase(IServiceScope scope, IEnumerable<Dev> devsFromSource)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<DesafioContext>();
        var existingDevs = await dbContext.Devs.AsNoTracking().ToListAsync();

        foreach (var dev in devsFromSource)
        {
            var existingDev = existingDevs.FirstOrDefault(d => d.Id == dev.Id);
            if (existingDev != null)
            {
                bool needsUpdate = existingDev.Avatar != dev.Avatar ||
                                   existingDev.Email != dev.Email ||
                                   existingDev.Squad != dev.Squad ||
                                   existingDev.Login != dev.Login ||
                                   existingDev.Name != dev.Name;

                if (needsUpdate)
                {
                    dbContext.Attach(existingDev);
                    dbContext.Entry(existingDev).State = EntityState.Modified;

                    existingDev.Avatar = dev.Avatar;
                    existingDev.Email = dev.Email;
                    existingDev.Squad = dev.Squad;
                    existingDev.Login = dev.Login;
                    existingDev.Name = dev.Name;
                }
            }
            else
            {
                dbContext.Devs.Add(dev);
            }
        }

        var devsToRemove = existingDevs.Where(d => !devsFromSource.Any(ds => ds.Id == d.Id)).ToList();

        if (devsToRemove?.Any() == true)
            dbContext.Devs.RemoveRange(devsToRemove);

        await dbContext.SaveChangesAsync();
    }
}
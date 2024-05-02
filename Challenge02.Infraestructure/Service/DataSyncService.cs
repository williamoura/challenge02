using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Challenge02.Domain.Models;
using Challenge02.Infraestructure.Repository;
using ILogger = Serilog.ILogger;

public class DataSyncService : IDataSyncService
{
    private readonly DesafioContext _context;
    private readonly IMongoCollection<Dev> _devsCollection;
    private readonly ILogger _logger;

    public DataSyncService(
            DesafioContext context,
            IMongoCollection<Dev> devsCollection,
            ILogger logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _devsCollection = devsCollection ?? throw new ArgumentNullException(nameof(devsCollection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SyncMongoDatabase(IEnumerable<Dev> devsFromSource)
    {
        _logger.Information("Starting synchronization of MongoDB database.");

        var existingDevs = await _devsCollection
                            .Find(_ => true)
                            .ToListAsync();

        _logger.Information("Retrieved {Count} existing developers from database.", existingDevs.Count);

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
                    _logger.Information("Developer with ID {DevId} requires an update.", dev.Id);

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
                _logger.Information("Adding new developer with ID {DevId}.", dev.Id);
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
            _logger.Information("Preparing to write {UpdateCount} updates to the database.", updates.Count);

            const int batchSize = 100;
            for (int i = 0; i < updates.Count; i += batchSize)
            {
                var batch = updates.Skip(i).Take(batchSize).ToList();
                await _devsCollection.BulkWriteAsync(batch);
            }
        }
        _logger.Information("MongoDB database synchronization complete.");
    }

    public async Task SyncSqlDatabase(IEnumerable<Dev> devsFromSource)
    {
        _logger.Information("Starting synchronization of SQL database.");

        var existingDevs = await _context.Devs.AsNoTracking().ToListAsync();

        _logger.Information("Retrieved {Count} existing developers from SQL database.", existingDevs.Count);

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
                    _logger.Information("Developer with ID {DevId} requires an update.", dev.Id);

                    _context.Attach(existingDev);
                    _context.Entry(existingDev).State = EntityState.Modified;

                    existingDev.Avatar = dev.Avatar;
                    existingDev.Email = dev.Email;
                    existingDev.Squad = dev.Squad;
                    existingDev.Login = dev.Login;
                    existingDev.Name = dev.Name;
                }
            }
            else
            {
                _logger.Information("Adding new developer with ID {DevId} to SQL database.", dev.Id);

                _context.Devs.Add(dev);
            }
        }

        var devsToRemove = existingDevs.Where(d => !devsFromSource.Any(ds => ds.Id == d.Id)).ToList();

        if (devsToRemove?.Any() == true)
        {
            _logger.Information("Preparing to remove {RemoveCount} developers from SQL database.", devsToRemove.Count);
            _context.Devs.RemoveRange(devsToRemove);
        }

        await _context.SaveChangesAsync();

        _logger.Information("SQL database synchronization complete.");
    }
}
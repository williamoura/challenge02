using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;
using ILogger = Serilog.ILogger;

namespace Challenge02.Infraestructure.Repository
{
    public class MongoDevRepository : IWriteDevRepository, IReadDevRepository
    {
        private readonly IMongoCollection<Dev> _devsCollection;
        private readonly ILogger _logger;

        public MongoDevRepository(
            IMongoCollection<Dev> devsCollection,
            ILogger logger)
        {
            _devsCollection = devsCollection ?? throw new ArgumentNullException(nameof(devsCollection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public bool CanExecute(RepositoryType type) => type == RepositoryType.Mongo;

        public async Task<IEnumerable<Dev>> GetAllAsync()
        {
            _logger.Information($"{nameof(MongoDevRepository)} - Getting all developers");
            var result = await _devsCollection.Find(dev => true).ToListAsync();
            _logger.Information($"{nameof(MongoDevRepository)} - Retrieved {result.Count} developers");
            return result;
        }

        public async Task<Dev> GetByIdAsync(int id)
        {
            _logger.Information($"{nameof(MongoDevRepository)} - Getting developer with ID: {id}");
            var result = await _devsCollection.Find(dev => dev.Id == id).FirstOrDefaultAsync();
            _logger.Information($"{nameof(MongoDevRepository)} - {(result != null ? $"Found developer with ID: {id}" : $"No developer found with ID: {id}")}");
            return result;
        }

        public async Task AddAsync(Dev dev)
        {
            _logger.Information($"{nameof(MongoDevRepository)} - Adding new developer");
            await _devsCollection.InsertOneAsync(dev);
            _logger.Information($"{nameof(MongoDevRepository)} - Developer added with ID: {dev.Id}");
        }

        public async Task UpdateAsync(Dev dev)
        {
            _logger.Information($"{nameof(MongoDevRepository)} - Updating developer with ID: {dev.Id}");

            var filter = Builders<Dev>.Filter.Eq(d => d.Id, dev.Id);

            var update = Builders<Dev>.Update
                                .Set(d => d.Avatar, dev.Avatar)
                                .Set(d => d.Email, dev.Email)
                                .Set(d => d.Squad, dev.Squad)
                                .Set(d => d.Login, dev.Login)
                                .Set(d => d.Name, dev.Name);

            var result = await _devsCollection.UpdateOneAsync(filter, update);
            _logger.Information($"{nameof(MongoDevRepository)} - {(result.IsAcknowledged ? $"Developer updated with ID: {dev.Id}" : $"Update failed for developer with ID: {dev.Id}")}");
        }

    }
}

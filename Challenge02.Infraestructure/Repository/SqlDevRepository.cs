using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;
using ILogger = Serilog.ILogger;

namespace Challenge02.Infraestructure.Repository
{
    public class SqlDevRepository : IWriteDevRepository, IReadDevRepository
    {
        private readonly ILogger _logger;
        private readonly DesafioContext _context;
        private readonly IDevIdProvider _devIdProvider;

        public SqlDevRepository(
            DesafioContext context,
            IDevIdProvider devIdProvider,
            ILogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _devIdProvider = devIdProvider ?? throw new ArgumentNullException(nameof(devIdProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool CanExecute(RepositoryType type) => type == RepositoryType.Sql;

        public async Task<IEnumerable<Dev>> GetAllAsync()
        {
            try
            {
                _logger.Information($"{nameof(SqlDevRepository)} - Retrieving all developers");
                var devs = await _context.Devs.AsNoTracking().ToListAsync();
                _logger.Information($"Retrieved {devs.Count} developers");
                return devs;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(SqlDevRepository)} - Error retrieving all developers");
                throw;
            }
        }

        public async Task<Dev?> GetByIdAsync(int id)
        {
            try
            {
                _logger.Information($"{nameof(SqlDevRepository)} - Retrieving developer with ID: {id}");
                var dev = await _context.Devs.FindAsync(id);
                if (dev == null)
                {
                    _logger.Warning($"{nameof(SqlDevRepository)} - Developer with ID: {id} not found");
                }
                else
                {
                    _logger.Information($"{nameof(SqlDevRepository)} - Developer with ID: {id} retrieved");
                }
                return dev;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(SqlDevRepository)} - Error retrieving developer with ID: {id}");
                throw;
            }
        }

        public async Task AddAsync(Dev dev)
        {
            try
            {
                dev.Id = await _devIdProvider.GetNextIdAsync();

                _logger.Information($"{nameof(SqlDevRepository)} - Adding new developer with ID: {dev.Id}");
                _context.Devs.Add(dev);
                await _context.SaveChangesAsync();
                _logger.Information($"{nameof(SqlDevRepository)} - Developer added with ID: {dev.Id}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(SqlDevRepository)} - Error adding new developer");
                throw;
            }
        }

        public async Task UpdateAsync(Dev dev)
        {
            try
            {
                var existingDev = await _context.Devs
                    .Where(d => d.Id == dev.Id)
                    .Select(d => new { d.CreatedAt })
                    .FirstOrDefaultAsync();

                if (existingDev != null)
                {
                    _logger.Information($"{nameof(SqlDevRepository)} - Updating developer with ID: {dev.Id}");

                    var devToUpdate = new Dev
                    {
                        Id = dev.Id,
                        Avatar = dev.Avatar,
                        Name = dev.Name,
                        Email = dev.Email,
                        Login = dev.Login,
                        CreatedAt = existingDev.CreatedAt
                    };

                    _context.Devs.Attach(devToUpdate);
                    _context.Entry(devToUpdate).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
                    _logger.Information($"{nameof(SqlDevRepository)} - Developer updated with ID: {dev.Id}");
                }
                else
                {
                    _logger.Information($"{nameof(SqlDevRepository)} - Developer not found with ID: {dev.Id}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(SqlDevRepository)} - Error updating developer with ID: {dev.Id}");
                throw;
            }

        }
    }
}

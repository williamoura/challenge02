using Challenge02.Domain.Models;

public interface IDataSyncService
{
    Task SyncMongoDatabase(IEnumerable<Dev> devsFromSource);
    Task SyncSqlDatabase(IEnumerable<Dev> devsFromSource);
}
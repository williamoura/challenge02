using Challenge02.Domain.Models;

namespace Challenge02.Domain.Interfaces
{
    public interface IWriteDevRepository
    {
        bool CanExecute(RepositoryType type);
        Task AddAsync(Dev dev);
        Task UpdateAsync(Dev dev);
    }
}

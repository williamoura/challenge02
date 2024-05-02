using Challenge02.Domain.Models;

namespace Challenge02.Domain.Interfaces
{
    public interface IReadDevRepository
    {
        bool CanExecute(RepositoryType type);
        Task<IEnumerable<Dev>> GetAllAsync();
        Task<Dev?> GetByIdAsync(int id);
    }
}

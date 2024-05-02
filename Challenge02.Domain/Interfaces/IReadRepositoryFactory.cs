using Challenge02.Domain.Models;

namespace Challenge02.Domain.Interfaces
{
    public interface IReadRepositoryFactory
    {
        IReadDevRepository GetReadRepository(RepositoryType repositoryType);
    }
}

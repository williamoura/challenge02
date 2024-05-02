using Challenge02.Domain.Models;

namespace Challenge02.Domain.Interfaces
{
    public interface IWriteRepositoryFactory
    {
        IWriteDevRepository GetWriteRepository(RepositoryType repositoryType);
    }
}

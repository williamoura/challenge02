using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;

namespace Challenge02.Infraestructure.Repository.Factories
{
    public class WriteRepositoryFactory : IWriteRepositoryFactory
    {
        private readonly IEnumerable<IWriteDevRepository> _repositories;

        public WriteRepositoryFactory(
            IEnumerable<IWriteDevRepository> repositories)
        {
            _repositories = repositories ?? throw new ArgumentNullException(nameof(repositories));
        }

        public IWriteDevRepository GetWriteRepository(RepositoryType repositoryType)
        {
            var repos = _repositories.FirstOrDefault(x => x.CanExecute(repositoryType));

            return repos ?? throw new InvalidOperationException($"No repository found for {repositoryType}");
        }
    }
}

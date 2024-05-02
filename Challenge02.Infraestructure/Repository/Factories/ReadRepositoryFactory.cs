using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;

namespace Challenge02.Infraestructure.Repository.Factories
{
    public class ReadRepositoryFactory : IReadRepositoryFactory
    {
        private readonly IEnumerable<IReadDevRepository> _repositories;

        public ReadRepositoryFactory(
            IEnumerable<IReadDevRepository> repositories)
        {
            _repositories = repositories ?? throw new ArgumentNullException(nameof(repositories));
        }

        public IReadDevRepository GetReadRepository(RepositoryType repositoryType)
        {
            var repos = _repositories.FirstOrDefault(x => x.CanExecute(repositoryType));

            return repos ?? throw new InvalidOperationException($"No repository found for {repositoryType}");
        }
    }
}

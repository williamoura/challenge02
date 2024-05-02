using MediatR;
using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;
using Challenge02.Domain.Queries;

namespace Challenge02.Infraestructure.Handlers.Queries
{
    public class GetAllDevsHandler : IRequestHandler<GetAllDevsQuery, IEnumerable<Dev>>
    {
        private readonly IReadRepositoryFactory _repositoryFactory;

        public GetAllDevsHandler(
            IReadRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
        }

        public async Task<IEnumerable<Dev>> Handle(
            GetAllDevsQuery query,
            CancellationToken cancellationToken)
        {
            var repository = _repositoryFactory.GetReadRepository(RepositoryType.Mongo);

            var devs = await repository.GetAllAsync();

            return devs;
        }
    }
}

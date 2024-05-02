using MediatR;
using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;
using Challenge02.Domain.Queries;

namespace Challenge02.Infraestructure.Handlers.Queries
{
    public class GetDevByIdHandler : IRequestHandler<GetDevByIdQuery, Dev>
    {
        private readonly IReadRepositoryFactory _repositoryFactory;

        public GetDevByIdHandler(
            IReadRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
        }

        public async Task<Dev> Handle(
            GetDevByIdQuery query,
            CancellationToken cancellationToken)
        {
            var repository = _repositoryFactory.GetReadRepository(RepositoryType.Mongo);

            var dev = await repository.GetByIdAsync(query.Id);

            return dev;
        }
    }
}

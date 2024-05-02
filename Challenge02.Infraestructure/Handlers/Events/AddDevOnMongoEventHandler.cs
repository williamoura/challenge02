using AutoMapper;
using Challenge02.Domain.Events;
using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;
using MediatR;

namespace Challenge02.Infraestructure.Handlers.Events
{
    public class AddDevOnMongoEventHandler : INotificationHandler<AddDevEvent>
    {
        private readonly IWriteRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;

        public AddDevOnMongoEventHandler(
            IWriteRepositoryFactory repositoryFactory,
            IMapper mapper)
        {
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Handle(AddDevEvent notification, CancellationToken cancellationToken)
        {
            var repository = _repositoryFactory.GetWriteRepository(RepositoryType.Mongo);

            var dev = _mapper.Map<Dev>(notification);

            await repository.AddAsync(dev);
        }
    }
}

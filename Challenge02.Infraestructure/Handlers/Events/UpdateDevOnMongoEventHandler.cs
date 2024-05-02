using AutoMapper;
using Challenge02.Domain.Events;
using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;
using MediatR;

namespace Challenge02.Infraestructure.Handlers.Events
{
    public class UpdateDevOnMongoEventHandler : INotificationHandler<UpdateDevEvent>
    {
        private readonly IWriteRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;

        public UpdateDevOnMongoEventHandler(
            IWriteRepositoryFactory repositoryFactory,
            IMapper mapper)
        {
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Handle(UpdateDevEvent notification, CancellationToken cancellationToken)
        {
            var dev = _mapper.Map<Dev>(notification);

            var repository = _repositoryFactory.GetWriteRepository(RepositoryType.Mongo);

            await repository.UpdateAsync(dev);
        }
    }
}

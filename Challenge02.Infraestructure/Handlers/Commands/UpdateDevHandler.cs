using AutoMapper;
using Challenge02.Domain.Commands;
using Challenge02.Domain.Events;
using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;
using Challenge02.Infraestructure.Messaging;
using MediatR;

namespace Challenge02.Infraestructure.Handlers.Commands
{
    public class UpdateDevHandler : IRequestHandler<UpdateDevCommand>
    {
        private readonly IWriteRepositoryFactory _repositoryFactory;
        private readonly IServiceBusMessageSender _messageSender;
        private readonly IMapper _mapper;

        public UpdateDevHandler(
            IWriteRepositoryFactory repositoryFactory,
            IServiceBusMessageSender messageSender,
            IMapper mapper)
        {
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Unit> Handle(
            UpdateDevCommand command,
            CancellationToken cancellationToken)
        {
            var dev = _mapper.Map<Dev>(command);

            var repository = _repositoryFactory.GetWriteRepository(RepositoryType.Sql);

            await repository.UpdateAsync(dev);

            var updateEvent = _mapper.Map<UpdateDevEvent>(dev);

            await _messageSender.SendMessageAsync(updateEvent);

            return Unit.Value;
        }
    }
}

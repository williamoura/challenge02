using AutoMapper;
using Challenge02.Domain.Commands;
using Challenge02.Domain.Events;
using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;
using Challenge02.Infraestructure.Messaging;
using MediatR;
using ILogger = Serilog.ILogger;

namespace Challenge02.Infraestructure.Handlers.Commands
{
    public class AddDevHandler : IRequestHandler<AddDevCommand, Dev>
    {
        private readonly IWriteRepositoryFactory _repositoryFactory;
        private readonly IServiceBusMessageSender _messageSender;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AddDevHandler(
            IWriteRepositoryFactory repositoryFactory,
            IServiceBusMessageSender messageSender,
            IMapper mapper,
            ILogger logger)
        {
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Dev> Handle(
            AddDevCommand command,
            CancellationToken cancellationToken)
        {
            var dev = _mapper.Map<Dev>(command);

            dev.CreatedAt = DateTime.UtcNow;

            var repository = _repositoryFactory.GetWriteRepository(RepositoryType.Sql);

            await repository.AddAsync(dev);

            var addEvent = _mapper.Map<AddDevEvent>(dev);

            await _messageSender.SendMessageAsync(addEvent);

            return dev;
        }
    }
}

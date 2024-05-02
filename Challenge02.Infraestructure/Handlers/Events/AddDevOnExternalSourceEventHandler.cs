using AutoMapper;
using MediatR;
using Challenge02.Domain.Events;
using Challenge02.Domain.Utilities;
using Challenge02.Infraestructure.Clients.Abstractions;
using Challenge02.Infraestructure.Clients.Contracts;
using ILogger = Serilog.ILogger;

namespace Challenge02.Infraestructure.Handlers.Events
{
    public class AddDevOnExternalSourceEventHandler : INotificationHandler<AddDevEvent>
    {
        private readonly IMockApiClient _mockApiClient;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AddDevOnExternalSourceEventHandler(
            IMockApiClient mockApiClient,
            IMapper mapper,
            ILogger logger)
        {
            _mockApiClient = mockApiClient ?? throw new ArgumentNullException(nameof(mockApiClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(AddDevEvent notification, CancellationToken cancellationToken)
        {
            var createRequest = _mapper.Map<CreateDevsRequest>(notification);

            var result = await _mockApiClient.SendAsync<EmptyResult>(createRequest);

            if (!result.Success)
            {
                _logger.Error("Error on create dev on mock api");
                throw new AggregateException(result.Errors);
            }
        }
    }
}

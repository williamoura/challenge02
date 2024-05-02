using AutoMapper;
using MediatR;
using Challenge02.Domain.Commands;
using Challenge02.Domain.Events;
using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Queries;
using ILogger = Serilog.ILogger;

namespace Challenge02.Infraestructure.Handlers.Events
{
    public class NormalizeDevEmailDomainsEventHandler : INotificationHandler<NormalizeDevEmailDomainEvent>
    {
        private readonly IEmailNormalizationService _emailNormalizationService;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public NormalizeDevEmailDomainsEventHandler(
            IEmailNormalizationService emailNormalizationService,
            IMapper mapper,
            IMediator mediator,
            ILogger logger)
        {
            _emailNormalizationService = emailNormalizationService ?? throw new ArgumentNullException(nameof(emailNormalizationService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(NormalizeDevEmailDomainEvent notification, CancellationToken cancellationToken)
        {
            var devs = await _mediator.Send(new GetAllDevsQuery(), cancellationToken);

            var updateTasks = new List<Task>();

            foreach (var dev in devs)
            {
                if (_emailNormalizationService.TryNormalizeEmailDomain(dev))
                {
                    var updateCommand = _mapper.Map<UpdateDevCommand>(dev);
                    updateTasks.Add(_mediator.Send(updateCommand, cancellationToken));
                }
            }

            await Task.WhenAll(updateTasks);

            _logger.Information("Dev email domains normalized successfully");
        }
    }
}

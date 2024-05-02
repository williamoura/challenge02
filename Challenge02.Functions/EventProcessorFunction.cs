using MediatR;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Challenge02.Domain.Events;
using System.Threading.Tasks;
using ILogger = Serilog.ILogger;

namespace Challenge02.Functions
{
    public class EventProcessorFunction
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public EventProcessorFunction(
            ILogger logger,
            IMediator mediator)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        [FunctionName("EventProcessor")]
        public async Task Run(
            [ServiceBusTrigger("desafio-queue", Connection = "ServiceBusConnection")] string message)
        {
            var baseEvent = JsonConvert.DeserializeObject<BaseEvent>(message);

            switch (baseEvent.EventType)
            {
                case NormalizeDevEmailDomainEvent.@event:
                    var normalizeEmailEvent = JsonConvert.DeserializeObject<NormalizeDevEmailDomainEvent>(message);
                    await _mediator.Publish(normalizeEmailEvent);
                    break;
                case UpdateDevEvent.@event:
                    var updateDevEvent = JsonConvert.DeserializeObject<UpdateDevEvent>(message);
                    await _mediator.Publish(updateDevEvent);
                    break;
                case AddDevEvent.@event:
                    var addDevEvent = JsonConvert.DeserializeObject<AddDevEvent>(message);
                    await _mediator.Publish(addDevEvent);
                    break;
                default:
                    _logger.Warning($"Event type {baseEvent.EventType} is not implemented");
                    break;
            }
        }
    }

}

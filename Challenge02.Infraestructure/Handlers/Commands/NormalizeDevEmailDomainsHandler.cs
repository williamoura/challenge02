using MediatR;
using Challenge02.Domain.Commands;
using Challenge02.Domain.Events;
using Challenge02.Infraestructure.Messaging;

namespace Challenge02.Infraestructure.Handlers.Commands
{
    public class NormalizeDevEmailDomainsHandler : IRequestHandler<NormalizeDevEmailDomainsCommand>
    {
        private readonly IServiceBusMessageSender _messageSender;

        public NormalizeDevEmailDomainsHandler(
            IServiceBusMessageSender messageSender)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        public async Task<Unit> Handle(
            NormalizeDevEmailDomainsCommand command,
            CancellationToken cancellationToken)
        {
            await _messageSender.SendMessageAsync(new NormalizeDevEmailDomainEvent());

            return Unit.Value;
        }
    }
}

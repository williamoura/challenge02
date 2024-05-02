using MediatR;

namespace Challenge02.Domain.Events
{
    public class NormalizeDevEmailDomainEvent : BaseEvent, INotification
    {
        public const string @event = "normalize-dev-email-domain-event";
        public NormalizeDevEmailDomainEvent() : base(@event)
        {
        }
    }
}

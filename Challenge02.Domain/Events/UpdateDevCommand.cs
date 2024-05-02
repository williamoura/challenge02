using MediatR;

namespace Challenge02.Domain.Events
{
    public class UpdateDevEvent : BaseEvent, INotification
    {
        public const string @event = "update-dev-event";
        public UpdateDevEvent() : base(@event)
        {
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? Squad { get; set; }
        public string? Login { get; set; }
        public string? Email { get; set; }
    }
}

using MediatR;

namespace Challenge02.Domain.Events
{
    public class AddDevEvent : BaseEvent, INotification
    {
        public const string @event = "add-dev-event";
        public AddDevEvent() : base(@event)
        {
        }

        public string Id { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; } = null!;
        public string? Avatar { get; set; }
        public string? Squad { get; set; }
        public string Login { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}

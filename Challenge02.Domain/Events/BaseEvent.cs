namespace Challenge02.Domain.Events
{
    public class BaseEvent
    {
        public BaseEvent() { }
        public string EventType { get; set; }

        protected BaseEvent(string eventType)
        {
            EventType = eventType;
        }
    }
}
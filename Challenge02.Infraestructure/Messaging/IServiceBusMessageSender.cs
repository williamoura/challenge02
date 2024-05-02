namespace Challenge02.Infraestructure.Messaging
{
    public interface IServiceBusMessageSender
    {
        Task SendMessageAsync<T>(T messageObject);
    }
}

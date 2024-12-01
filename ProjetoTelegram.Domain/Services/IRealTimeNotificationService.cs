namespace ProjetoTelegram.Domain.Services
{
    public interface IRealTimeNotificationService<TMessage> : INotificationService<TMessage>
        where TMessage : IRealTimeNotificationMessage
    {
    }
}

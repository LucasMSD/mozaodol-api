namespace ProjetoTelegram.Domain.Services
{
    public interface IPushNotificationService<TMessage> : INotificationService<TMessage>
        where TMessage : IPushNotificationMessage
    {
    }
}

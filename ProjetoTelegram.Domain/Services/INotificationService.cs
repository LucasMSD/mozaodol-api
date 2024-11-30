namespace ProjetoTelegram.Domain.Services
{
    public interface INotificationService<TMessage> where TMessage : INotificationMessage
    {
        Task Notify(IEnumerable<string> usersToSend, TMessage notification);
    }
}

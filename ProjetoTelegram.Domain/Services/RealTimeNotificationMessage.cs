namespace ProjetoTelegram.Domain.Services
{
    public class RealTimeNotificationMessage : IRealTimeNotificationMessage
    {
        public object Content { get; set; }
        public string ChannelId { get; set; }
    }
}

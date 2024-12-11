namespace Mozaodol.Domain.Services
{
    public class PushNotificationMessage : IPushNotificationMessage
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Priority { get; set; }
        public string Content { get; set; }
        public string ChannelId { get; set; }
    }
}

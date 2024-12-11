namespace Mozaodol.Domain.Services
{
    public interface IPushNotificationMessage : INotificationMessage
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Priority { get; set; }
        public string Content { get; set; }
    }
}

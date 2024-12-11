namespace Mozaodol.Domain.Services
{
    public interface IRealTimeNotificationMessage : INotificationMessage
    {
        public object Content { get; set; }
    }
}

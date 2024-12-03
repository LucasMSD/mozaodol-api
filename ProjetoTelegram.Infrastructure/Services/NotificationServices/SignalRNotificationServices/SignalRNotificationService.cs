using Microsoft.AspNetCore.SignalR;
using ProjetoTelegram.Domain.Services;

namespace ProjetoTelegram.Infrastructure.Services.NotificationServices.SignalRNotificationServices
{
    public class SginalRNotificationServiceServices<TMessage, THub> : IRealTimeNotificationService<TMessage>
        where TMessage : IRealTimeNotificationMessage
        where THub : Hub
    {
        private readonly IHubContext<THub> _hubContext;

        public SginalRNotificationServiceServices(
            IHubContext<THub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Notify(IEnumerable<string> usersToSend, TMessage notification)
        {
            var clients = usersToSend.Any() ? _hubContext.Clients.Users(usersToSend) : _hubContext.Clients.All;

            await clients.SendAsync(notification.ChannelId, notification.Content);
        }
    }
}

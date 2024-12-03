using Microsoft.AspNetCore.SignalR;
using ProjetoTelegram.Domain.Services;

namespace ProjetoTelegram.Infrastructure.Services.NotificationServices.SignalRNotificationServices
{
    public class SignalRNotificationService<THub> : IRealTimeNotificationService
        where THub : Hub
    {
        private readonly IHubContext<THub> _hubContext;

        public SignalRNotificationService(
            IHubContext<THub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Notify(IEnumerable<string> usersToSend, IRealTimeNotificationMessage notification)
        {
            var clients = usersToSend.Any() ? _hubContext.Clients.Users(usersToSend) : _hubContext.Clients.All;

            await clients.SendAsync(notification.ChannelId, notification.Content);
        }
    }
}

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

        public async Task AddConnectionToGroup(string connectionId, string groupName)
            => await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);

        public async Task AddConnectionToGroup(IEnumerable<string> connectionsIds, string groupName)
            => await Task.WhenAll(connectionsIds.Select(conId => _hubContext.Groups.AddToGroupAsync(conId, groupName)));

        public async Task AddConnectionToGroup(string connectionId, IEnumerable<string> groupsNames)
            => await Task.WhenAll(groupsNames.Select(group => _hubContext.Groups.AddToGroupAsync(connectionId, group)));

        public async Task Notify(IEnumerable<string> usersToSend, IRealTimeNotificationMessage notification)
        {
            var clients = usersToSend.Any() ? _hubContext.Clients.Users(usersToSend) : _hubContext.Clients.All;

            await clients.SendAsync(notification.ChannelId, notification.Content);
        }

        public async Task Notify(string userId, IRealTimeNotificationMessage message)
            => await _hubContext.Clients.User(userId).SendAsync(message.ChannelId, message.Content);

        public async Task NotifyExcept(string excludedConnection, IRealTimeNotificationMessage message)
            => await _hubContext.Clients.AllExcept(excludedConnection).SendAsync(message.ChannelId, message.Content);
        public async Task NotifyGroup(string groupName, IRealTimeNotificationMessage message)
            => await _hubContext.Clients.Groups(groupName).SendAsync(message.ChannelId, message.Content);

        public async Task NotifyGroupExcept(string groupName, string exceptConnectionId, IRealTimeNotificationMessage message)
            => await _hubContext.Clients.GroupExcept(groupName, exceptConnectionId).SendAsync(message.ChannelId, message.Content);

        public async Task NotifyGroupExcept(string groupName, IEnumerable<string> excludedConnectionsIds, IRealTimeNotificationMessage message)
            => await _hubContext.Clients.GroupExcept(groupName, excludedConnectionsIds).SendAsync(message.ChannelId, message.Content);

        public async Task NotifyGroupExcept(IEnumerable<string> groupsNames, string excludedConnectionId, IRealTimeNotificationMessage message)
            => await Task.WhenAll(groupsNames.Select(group => NotifyGroupExcept(group, excludedConnectionId, message)));
    }
}

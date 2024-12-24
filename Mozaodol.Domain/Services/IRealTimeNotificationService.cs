namespace Mozaodol.Domain.Services
{
    public interface IRealTimeNotificationService : INotificationService<IRealTimeNotificationMessage>
    {
        Task AddConnectionToGroup(string connectionId, string groupName);
        Task AddConnectionToGroup(IEnumerable<string> connectionsIds, string groupName);
        Task AddConnectionToGroup(string connectionId, IEnumerable<string> groupsNames);

        Task Notify(string userId, IRealTimeNotificationMessage message);
        Task NotifyExcept(string excludedConnection, IRealTimeNotificationMessage message);
        Task NotifyGroup(string groupName, IRealTimeNotificationMessage message);
        Task NotifyGroupExcept(string groupName, string excludedConnectionId, IRealTimeNotificationMessage message);
        Task NotifyGroupExcept(string groupName, IEnumerable<string> excludedConnectionsIds, IRealTimeNotificationMessage message);
        Task NotifyGroupExcept(IEnumerable<string> groupsNames, string excludedConnectionId, IRealTimeNotificationMessage message);
    }
}

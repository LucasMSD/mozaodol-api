using Expo.Server.Client;
using Expo.Server.Models;
using Mozaodol.Domain.Services;

namespace Mozaodol.Infrastructure.Services.NotificationServices.ExpoPushNotificationServices
{
    public class ExpoPushNotificationService : IPushNotificationService
    {
        public async Task Notify(IEnumerable<string> usersToSend, IPushNotificationMessage notification)
        {
            var expoSDKClient = new PushApiClient();
            var pushTicketReq = new PushTicketRequest()
            {
                PushTo = usersToSend.ToList(),
                PushBody = notification.Content,
                PushTitle = notification.Title,
                PushChannelId = notification.ChannelId,
                PushSubTitle = notification.SubTitle,
                PushPriority = notification.Priority
            };
            await expoSDKClient.PushSendAsync(pushTicketReq);
        }
    }
}

﻿using Expo.Server.Client;
using Expo.Server.Models;
using ProjetoTelegram.Domain.Services;

namespace ProjetoTelegram.Infrastructure.Services.NotificationServices.ExpoPushNotificationServices
{
    public class ExpoPushNotificationService<TMessage> : IPushNotificationService<TMessage>
        where TMessage : IPushNotificationMessage
    {

        public async Task Notify(IEnumerable<string> usersToSend, TMessage notification)
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
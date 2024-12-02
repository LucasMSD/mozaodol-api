﻿using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using ProjetoTelegram.Domain.Repositories.ChatRepositories;
using ProjetoTelegram.Domain.Services;
using SharpCompress.Readers;
using System.Text.Json;

namespace ProjetoTelegram.Application.UseCases.ChatUseCases
{
    public class OnDisconnectedUseCase :
        DefaultUseCase<Exception?, object?>,
        IOnDisconnectedUseCase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IChatRepository _chatRepository;
        private readonly INotificationService<IRealTimeNotificationMessage> _notificationService;

        public OnDisconnectedUseCase(
            IDistributedCache distributedCache,
            IChatRepository chatRepository,
            INotificationService<IRealTimeNotificationMessage> notificationService)
        {
            _distributedCache = distributedCache;
            _chatRepository = chatRepository;
            _notificationService = notificationService;
        }

        public override async Task<object?> Handle(Exception? input, CancellationToken cancellationToken)
        {
            // todo: refatorar
            var userIdString = User.Id.ToString();
            var userStateJson = await _distributedCache.GetStringAsync(userIdString);

            if (string.IsNullOrEmpty(userStateJson)) return null;

            var userState = JsonSerializer.Deserialize<UserState>(userStateJson);
            userState.Connected = false;

            await _distributedCache.RemoveAsync(userIdString);
            await _distributedCache.SetStringAsync(userIdString, JsonSerializer.Serialize(userState));

            await _notificationService.Notify([], new RealTimeNotificationMessage
            {
                ChannelId = "UserOnlineStatus",
                Content = false
            });

            return null;
        }
    }
}

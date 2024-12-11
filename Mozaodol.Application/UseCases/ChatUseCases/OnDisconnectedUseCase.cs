using FluentResults;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using Mozaodol.Application.DTOs.UserDTOs;
using Mozaodol.Domain.Repositories.ChatRepositories;
using Mozaodol.Domain.Services;
using SharpCompress.Readers;
using System.Text.Json;

namespace Mozaodol.Application.UseCases.ChatUseCases
{
    public class OnDisconnectedUseCase :
        DefaultUseCase<Exception?, object?>,
        IOnDisconnectedUseCase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IChatRepository _chatRepository;
        private readonly IRealTimeNotificationService _notificationService;

        public OnDisconnectedUseCase(
            IDistributedCache distributedCache,
            IChatRepository chatRepository,
            IRealTimeNotificationService notificationService)
        {
            _distributedCache = distributedCache;
            _chatRepository = chatRepository;
            _notificationService = notificationService;
        }

        public override async Task<Result<object?>> Handle(Exception? input, CancellationToken cancellationToken)
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

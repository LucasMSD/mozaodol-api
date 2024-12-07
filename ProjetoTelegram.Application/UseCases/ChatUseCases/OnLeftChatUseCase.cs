using FluentResults;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using ProjetoTelegram.Domain.Repositories.ChatRepositories;
using ProjetoTelegram.Domain.Services;
using System.Data;
using System.Text.Json;

namespace ProjetoTelegram.Application.UseCases.ChatUseCases
{
    public class OnLeftChatUseCase :
        DefaultUseCase<object?, object?>,
        IOnLeftChatUseCase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IChatRepository _chatRepository;
        private readonly IRealTimeNotificationService _realTimeNotificationService;

        public OnLeftChatUseCase(
            IDistributedCache distributedCache,
            IChatRepository chatRepository,
            IRealTimeNotificationService realTimeNotificationService)
        {
            _distributedCache = distributedCache;
            _chatRepository = chatRepository;
            _realTimeNotificationService = realTimeNotificationService;
        }

        public override async Task<Result<object?>> Handle(object? input, CancellationToken cancellationToken)
        {
            var userIdString = User.Id.ToString();
            var userStateJson = await _distributedCache.GetStringAsync(userIdString);
            var userState = JsonSerializer.Deserialize<UserState>(userStateJson);

            userState.Connected = true;
            var openedChatId = userState.OpenedChatId;
            userState.OpenedChatId = null;

            await _distributedCache.RemoveAsync(userIdString);
            await _distributedCache.SetStringAsync(userIdString, JsonSerializer.Serialize(userState));

            await _realTimeNotificationService.NotifyExcept(User.Connection, new RealTimeNotificationMessage
            {
                ChannelId = "UserOnlineStatus",
                Content = false
            });

            return null;
        }
    }
}

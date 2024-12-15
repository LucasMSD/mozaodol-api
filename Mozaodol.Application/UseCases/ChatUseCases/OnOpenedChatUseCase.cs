using FluentResults;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson.Serialization.IdGenerators;
using Mozaodol.Application.DTOs.ChatDTOs;
using Mozaodol.Application.DTOs.UserDTOs;
using Mozaodol.Domain.Repositories.ChatRepositories;
using Mozaodol.Domain.Services;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Mozaodol.Application.UseCases.ChatUseCases
{
    public class OnOpenedChatUseCase :
        DefaultUseCase<OnOpenedChatDTO, object?>,
        IOnOpenedChatUseCase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRealTimeNotificationService _realTimeNotificationService;
        private readonly IChatRepository _chatRepository;

        public OnOpenedChatUseCase(
            IDistributedCache distributedCache,
            IRealTimeNotificationService realTimeNotificationService,
            IChatRepository chatRepository)
        {
            _distributedCache = distributedCache;
            _realTimeNotificationService = realTimeNotificationService;
            _chatRepository = chatRepository;
        }

        public override async Task<Result<object?>> Handle(OnOpenedChatDTO input, CancellationToken cancellationToken)
        {
            var userIdString = User.Id.ToString();
            var userStateJson = await _distributedCache.GetStringAsync(userIdString);
            var userState = JsonSerializer.Deserialize<UserState>(userStateJson);

            userState.Connected = true;
            userState.OpenedChatId = input.ChatId.ToString();

            await _distributedCache.RemoveAsync(userIdString);
            await _distributedCache.SetStringAsync(userIdString, JsonSerializer.Serialize(userState));
            await _realTimeNotificationService.NotifyGroupExcept(input.ChatId.ToString(), User.Connection, new RealTimeNotificationMessage
            {
                ChannelId = "UserOnlineStatus",
                Content = true
            });

            var chat = await _chatRepository.Get(input.ChatId);

            var otherUser = chat.UsersIds.FirstOrDefault(x => x != User.Id);
            if (otherUser == default) return null;
            var otherUserStateJson = await _distributedCache.GetStringAsync(otherUser.ToString());
            if (string.IsNullOrEmpty(otherUserStateJson)) return null;
            var otherUserState = JsonSerializer.Deserialize<UserState>(otherUserStateJson);
            await _realTimeNotificationService.Notify(User.Id.ToString(), new RealTimeNotificationMessage
            {
                ChannelId = "UserOnlineStatus",
                Content = otherUserState.Connected
            });

            // todo: pensar nos casos em vc entra no chat e a pessoa já está online

            return null;
        }
    }
}


using FluentResults;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using Mozaodol.Application.DTOs.UserDTOs;
using Mozaodol.Application.UseCases.UserUseCases;
using Mozaodol.Domain.Repositories.ChatRepositories;
using Mozaodol.Domain.Repositories.UserRepositories;
using Mozaodol.Domain.Services;
using System.Text.Json;

namespace Mozaodol.Application.UseCases.ChatUseCases
{
    public class OnConnectedUseCase :
        DefaultUseCase<object?, object?>,
        IOnConnectedUseCase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IGetUserDtoUseByIdUseCase _getUserDtoUseByIdUseCase;
        private readonly IChatRepository _chatRepository;
        private readonly IRealTimeNotificationService _realTimeNotificationService;
        private readonly IUserRepository  _userRepository;

        public OnConnectedUseCase(
            IDistributedCache distributedCache,
            IGetUserDtoUseByIdUseCase getUserDtoUseByIdUseCase,
            IRealTimeNotificationService realTimeNotificationService,
            IChatRepository chatRepository,
            IUserRepository userRepository)
        {
            _distributedCache = distributedCache;
            _getUserDtoUseByIdUseCase = getUserDtoUseByIdUseCase;
            _realTimeNotificationService = realTimeNotificationService;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public override async Task<Result<object?>> Handle(object? input, CancellationToken cancellationToken)
        {
            var userIdString = User.Id.ToString();
            var userSateJson = await _distributedCache.GetStringAsync(userIdString);

            if (string.IsNullOrEmpty(userSateJson))
            {
                var getUserResult = await _getUserDtoUseByIdUseCase.Handle(new ObjectId(userIdString), cancellationToken);
                if (getUserResult.IsFailed) return null;

                userSateJson = JsonSerializer.Serialize(new UserState()
                {
                    UserId = getUserResult.Value._id.ToString(),
                    PushToken = getUserResult.Value.PushToken,
                    Connected = true
                });

                await _distributedCache.SetStringAsync(userIdString, userSateJson);
            }
            var userState = JsonSerializer.Deserialize<UserState>(userSateJson);
            userState.Connected = true;

            await _distributedCache.RemoveAsync(userIdString);
            await _distributedCache.SetStringAsync(userIdString, JsonSerializer.Serialize(userState));

            var user = await _userRepository.Get(User.Id);
            var chats = user.ChatsIds.Select(x => x.ToString());
            await _realTimeNotificationService.AddConnectionToGroup(User.Connection, chats);
            if (string.IsNullOrEmpty(userState.OpenedChatId)) return null;
            await _realTimeNotificationService.NotifyGroupExcept(userState.OpenedChatId, User.Connection, new RealTimeNotificationMessage
            {
                ChannelId = "UserOnlineStatus",
                Content = true
            });

            return null;
        }
    }
}

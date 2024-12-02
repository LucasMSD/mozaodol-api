using Microsoft.Extensions.Caching.Distributed;
using ProjetoTelegram.Application.DTOs.ChatDTOs;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using ProjetoTelegram.Domain.Repositories.ChatRepositories;
using ProjetoTelegram.Domain.Services;
using System.Text.Json;

namespace ProjetoTelegram.Application.UseCases.ChatUseCases
{
    public class OnOpenedChatUseCase :
        DefaultUseCase<OnOpenedChatDTO, object?>,
        IOnOpenedChatUseCase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly INotificationService<IRealTimeNotificationMessage> _realTimeNotificationService;
        private readonly IChatRepository _chatRepository;

        public OnOpenedChatUseCase(
            IDistributedCache distributedCache,
            INotificationService<IRealTimeNotificationMessage> realTimeNotificationService,
            IChatRepository chatRepository)
        {
            _distributedCache = distributedCache;
            _realTimeNotificationService = realTimeNotificationService;
            _chatRepository = chatRepository;
        }

        public override async Task<object?> Handle(OnOpenedChatDTO input, CancellationToken cancellationToken)
        {
            var userIdString = User.Id.ToString();
            var userStateJson = await _distributedCache.GetStringAsync(userIdString);
            var userState = JsonSerializer.Deserialize<UserState>(userStateJson);

            userState.Connected = true;
            userState.OpenedChatId = input.ChatId.ToString();

            await _distributedCache.RemoveAsync(userIdString);
            await _distributedCache.SetStringAsync(userIdString, JsonSerializer.Serialize(userState));


            var chatResult = await _chatRepository.Get(input.ChatId);

            var usersToNotify = chatResult.Value.UsersIds.Where(userId => userId != User.Id).Select(x => x.ToString());
            await _realTimeNotificationService.Notify(usersToNotify, new RealTimeNotificationMessage
            {
                ChannelId = "UserOnlineStatus",
                Content = true
            });

            // todo: pensar nos casos em vc entra no chat e a pessoa já está online

            return null;
        }
    }
}

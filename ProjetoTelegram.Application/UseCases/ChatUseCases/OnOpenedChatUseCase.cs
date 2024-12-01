using Microsoft.Extensions.Caching.Distributed;
using ProjetoTelegram.Application.DTOs.ChatDTOs;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using System.Text.Json;

namespace ProjetoTelegram.Application.UseCases.ChatUseCases
{
    public class OnOpenedChatUseCase :
        DefaultUseCase<OnOpenedChatDTO, object?>,
        IOnOpenedChatUseCase
    {
        private readonly IDistributedCache _distributedCache;

        public OnOpenedChatUseCase(
            IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
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

            return null;
        }
    }
}

using Microsoft.Extensions.Caching.Distributed;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using System.Data;
using System.Text.Json;

namespace ProjetoTelegram.Application.UseCases.ChatUseCases
{
    public class OnLeftChatUseCase :
        DefaultUseCase<object?, object?>,
        IOnLeftChatUseCase
    {
        private readonly IDistributedCache _distributedCache;

        public OnLeftChatUseCase(
            IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public override async Task<object?> Handle(object? input, CancellationToken cancellationToken)
        {
            var userIdString = User.Id.ToString();
            var userStateJson = await _distributedCache.GetStringAsync(userIdString);
            var userState = JsonSerializer.Deserialize<UserState>(userStateJson);

            userState.Connected = true;
            userState.OpenedChatId = null;

            await _distributedCache.RemoveAsync(userIdString);
            await _distributedCache.SetStringAsync(userIdString, JsonSerializer.Serialize(userState));

            return null;
        }
    }
}

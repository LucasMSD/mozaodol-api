using Microsoft.Extensions.Caching.Distributed;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using System.Text.Json;

namespace ProjetoTelegram.Application.UseCases.ChatUseCases
{
    public class OnDisconnectedUseCase :
        DefaultUseCase<Exception?, object?>,
        IOnDisconnectedUseCase
    {
        private readonly IDistributedCache _distributedCache;

        public OnDisconnectedUseCase(
            IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
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

            return null;
        }
    }
}

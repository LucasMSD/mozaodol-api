
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using ProjetoTelegram.Application.UseCases.UserUseCases;
using System.Text.Json;

namespace ProjetoTelegram.Application.UseCases.ChatUseCases
{
    public class OnConnectedUseCase :
        DefaultUseCase<object?, object?>,
        IOnConnectedUseCase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IGetUserDtoUseByIdUseCase _getUserDtoUseByIdUseCase;

        public OnConnectedUseCase(
            IDistributedCache distributedCache,
            IGetUserDtoUseByIdUseCase getUserDtoUseByIdUseCase)
        {
            _distributedCache = distributedCache;
            _getUserDtoUseByIdUseCase = getUserDtoUseByIdUseCase;
        }

        public override async Task<object?> Handle(object? input, CancellationToken cancellationToken)
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
                return null;
            }
            var userState = JsonSerializer.Deserialize<UserState>(userSateJson);
            userState.Connected = true;

            await _distributedCache.RemoveAsync(userIdString);
            await _distributedCache.SetStringAsync(userIdString, JsonSerializer.Serialize(userState));

            return null;
        }
    }
}

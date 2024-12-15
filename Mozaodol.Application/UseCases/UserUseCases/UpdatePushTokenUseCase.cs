using FluentResults;
using Microsoft.Extensions.Caching.Distributed;
using Mozaodol.Application.DTOs.UserDTOs;
using Mozaodol.Domain.Repositories.UserRepositories;
using System.Text.Json;

namespace Mozaodol.Application.UseCases.UserUseCases
{
    public class UpdatePushTokenUseCase :
        DefaultUseCase<UpdatePushTokenDTO, object?>,
        IUpdatePushTokenUseCase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IUserRepository _userRepository;

        public UpdatePushTokenUseCase(
            IDistributedCache distributedCache,
            IUserRepository userRepository)
        {
            _distributedCache = distributedCache;
            _userRepository = userRepository;
        }

        public override async Task<Result<object?>> Handle(UpdatePushTokenDTO input, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Get(User.Id);
            if (user == null) return Result.Fail("Usuário não encontrado.");

            await _userRepository.UpdatePushToken(User.Id, input.PushToken);

            // todo: refatorar urgentemente
            var userStateJson = await _distributedCache.GetStringAsync(User.Id.ToString());
            var userState = JsonSerializer.Deserialize<UserState>(userStateJson);
            userState.PushToken = input.PushToken;

            await _distributedCache.RemoveAsync(User.Id.ToString());
            await _distributedCache.SetStringAsync(User.Id.ToString(), JsonSerializer.Serialize(userState));

            return Result.Ok();
        }
    }
}

using FluentResults;
using Microsoft.Extensions.Caching.Distributed;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using ProjetoTelegram.Domain.Repositories.UserRepositories;
using System.Text.Json;

namespace ProjetoTelegram.Application.UseCases.UserUseCases
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

        public override async Task<object?> Handle(UpdatePushTokenDTO input, CancellationToken cancellationToken)
        {
            var getUsersResult = await _userRepository.Get(User.Id);
            if (getUsersResult.IsFailed) return Result.Fail("Erro ao buscar lista de contatos.").WithErrors(getUsersResult.Errors);
            if (getUsersResult.Value == null) return Result.Fail("Usuário não encontrado.");

            var updateResult = await _userRepository.UpdatePushToken(User.Id, input.PushToken);
            if (updateResult.IsFailed) return Result.Fail("Erro ao atualizar push token.").WithErrors(updateResult.Errors);

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

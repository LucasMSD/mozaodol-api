using FluentResults;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using ProjetoTelegram.Domain.Repositories.UserRepositories;

namespace ProjetoTelegram.Application.UseCases.UserUseCases
{
    public class GetUserDtoUseByIdUseCase :
        DefaultUseCase<ObjectId, UserDto>,
        IGetUserDtoUseByIdUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUserDtoUseByIdUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public override async Task<Result<UserDto>> Handle(ObjectId input, CancellationToken cancellationToken)
        {
            var getUserResult = await _userRepository.Get(input);
            if (getUserResult.IsFailed) return Result.Fail("Erro ao buscar usuário.").WithErrors(getUserResult.Errors);

            return new UserDto
            {
                Name = getUserResult.Value.Name,
                Username = getUserResult.Value.Username,
                _id = getUserResult.Value._id,
                PushToken = getUserResult.Value.PushToken,
            };
        }
    }
}

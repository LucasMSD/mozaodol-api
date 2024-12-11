using FluentResults;
using Mozaodol.Application.DTOs.AuthDTOs;
using Mozaodol.Application.DTOs.UserDTOs;
using Mozaodol.Domain.Entities.UserEntities;
using Mozaodol.Domain.Repositories.UserRepositories;

namespace Mozaodol.Application.UseCases.Auth.AuthUseCases
{
    public class AuthSignupUseCase :
        DefaultUseCase<AuthSignupModel, UserDto>,
        IAuthSignupUseCase
    {
        private readonly IUserRepository _userRepository;

        public AuthSignupUseCase(
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public override async Task<Result<UserDto>> Handle(AuthSignupModel input, CancellationToken cancellationToken)
        {
            {
                var userExistResult = await _userRepository.Exists(input.Username);
                // validar se o username já existe
                if (userExistResult.IsFailed)
                {
                    return Result.Fail("Não foi possível verificar se já existe outro usuário com o mesmo Username.").WithErrors(userExistResult.Errors);
                }

                if (userExistResult.Value)
                {
                    return Result.Fail("Username já utilizado.");
                }

                // salvar o novo usuário
                var user = await _userRepository.Add(new User
                {
                    Name = input.Name,
                    Username = input.Username,
                    Password = input.Password,
                });

                return new UserDto
                {
                    _id = user.Value._id,
                    Name = user.Value.Name,
                    PushToken = user.Value.PushToken,
                    Username = user.Value.Username
                };
            }
        }
    }
}

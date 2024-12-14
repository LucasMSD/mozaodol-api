using FluentResults;
using Mozaodol.Application.DTOs.AuthDTOs;
using Mozaodol.Application.DTOs.UserDTOs;
using Mozaodol.Application.Extensions.Results;
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
                var userAlreadyExists = await _userRepository.Exists(input.Username);

                if (userAlreadyExists)
                {
                    return Result.Fail("Username já utilizado.").SetStatusCode(409);
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
                    _id = user._id,
                    Name = user.Name,
                    PushToken = user.PushToken,
                    Username = user.Username
                }.ToResult(200);
            }
        }
    }
}

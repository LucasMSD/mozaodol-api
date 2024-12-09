using FluentResults;
using ProjetoTelegram.Application.DTOs.AuthDTOs;
using ProjetoTelegram.Application.Extensions.Results;
using ProjetoTelegram.Domain.Repositories.UserRepositories;
using ProjetoTelegram.Domain.Services;

namespace ProjetoTelegram.Application.UseCases.Auth.AuthUseCases
{
    public class AuthLoginUseCase :
        DefaultUseCase<AuthLoginModel, string>,
        IAuthLoginUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthLoginUseCase(
            IUserRepository userRepository,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public override async Task<Result<string>> Handle(AuthLoginModel input, CancellationToken cancellationToken)
        {
            var userResult = await _userRepository.GetByLogin(input.Username, input.Password);

            if (userResult.IsFailed)
            {
                return Result.Fail("Erro ao tentar realizar o login.").WithErrors(userResult.Errors);
            }

            if (userResult.Value == null)
            {
                return Result.Fail("Combinação de Username e senha incorretos.");
            }

            var token = _tokenService.GenerateToken(userResult.Value._id, userResult.Value.Username);

            return token.SetStatusCode(200);
        }
    }
}

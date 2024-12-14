using FluentResults;
using Mozaodol.Application.DTOs.AuthDTOs;
using Mozaodol.Application.Extensions.Results;
using Mozaodol.Domain.Repositories.UserRepositories;
using Mozaodol.Domain.Services;

namespace Mozaodol.Application.UseCases.Auth.AuthUseCases
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
            var user = await _userRepository.GetByLogin(input.Username, input.Password);
            if (user == null)
                return Result.Fail("Combinação de Username e senha incorretos.").SetStatusCode(401);

            var token = _tokenService.GenerateToken(user._id, user.Username);
            if (token.IsFailed)
                return Result.Fail("Houve um problmea interno").SetStatusCode(500);

            return token.SetStatusCode(200);
        }
    }
}

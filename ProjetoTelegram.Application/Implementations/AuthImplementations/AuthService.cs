using FluentResults;
using ProjetoTelegram.Application.DTOs.AuthDTOs;
using ProjetoTelegram.Application.Interfaces.AuthInterfaces;
using ProjetoTelegram.Domain.Entities.UserEntities;
using ProjetoTelegram.Domain.Repositories.UserRepositories;
using ProjetoTelegram.Domain.Services;

namespace ProjetoTelegram.Application.Implementations.AuthImplementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<Result<User>> Signup(AuthSignupModel signupModel)
        {
            var userExistResult = await _userRepository.Exists(signupModel.Username);
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
            return await _userRepository.Add(new User
            {
                Name = signupModel.Name,
                Username = signupModel.Username,
                Password = signupModel.Password,
            });
        }

        public async Task<Result<string>> Login(AuthLoginModel authLoginModel)
        {
            var userResult = await _userRepository.GetByLogin(authLoginModel.Username, authLoginModel.Password);

            if (userResult.IsFailed)
            {
                return Result.Fail("Erro ao tentar realizar o login.").WithErrors(userResult.Errors);
            }

            if (userResult.Value == null)
            {
                return Result.Fail("Combinação de Username e senha incorretos.");
            }

            return _tokenService.GenerateToken(userResult.Value._id);
        }
    }
}

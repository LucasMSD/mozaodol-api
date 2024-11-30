using FluentResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.AuthDTOs;
using ProjetoTelegram.Application.Interfaces.AuthInterfaces;
using ProjetoTelegram.Domain.Config.JwtConfig;
using ProjetoTelegram.Domain.Entities.UserEntities;
using ProjetoTelegram.Domain.Repositories.UserRepositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjetoTelegram.Application.Implementations.AuthImplementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings.Value;
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

            return GenerateToken(userResult.Value._id);
        }

        public string GenerateToken(ObjectId userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

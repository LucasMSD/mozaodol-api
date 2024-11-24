using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using ProjetoTelegram.Domain.Config.JwtConfig;
using ProjetoTelegram.Domain.Models.Auth;
using ProjetoTelegram.Domain.Models.User;
using ProjetoTelegram.Domain.Repositories.UserRepositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjetoTelegram.Domain.Services.AuthServices
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<User> Signup(AuthSignupModel signupModel)
        {
            // validar se o username já existe
            if (await _userRepository.Exists(signupModel.Username))
            {
                return null;
            }

            // salvar o novo usuário
            return await _userRepository.Add(new User
            {
                Name = signupModel.Name,
                Username = signupModel.Username,
                Password = signupModel.Password,
            });
        }

        public string Login(AuthLoginModel authLoginModel)
        {
            var user = _userRepository.GetByLogin(authLoginModel.Username, authLoginModel.Password);

            if (user == null)
            {
                return string.Empty;
            }

            return GenerateToken(user._id);
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

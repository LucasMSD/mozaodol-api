using MongoDB.Bson;
using ProjetoTelegram.Domain.Models.Auth;
using ProjetoTelegram.Domain.Models.User;

namespace ProjetoTelegram.Application.Interfaces.AuthInterfaces
{
    public interface IAuthService
    {
        public Task<User> Signup(AuthSignupModel signupModel);
        public string Login(AuthLoginModel authLoginModel);
        public string GenerateToken(ObjectId userId);
    }
}

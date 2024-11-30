using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.AuthDTOs;
using ProjetoTelegram.Domain.Entities.UserEntities;

namespace ProjetoTelegram.Application.Interfaces.AuthInterfaces
{
    public interface IAuthService
    {
        public Task<User> Signup(AuthSignupModel signupModel);
        public string Login(AuthLoginModel authLoginModel);
        public string GenerateToken(ObjectId userId);
    }
}

using FluentResults;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.AuthDTOs;
using ProjetoTelegram.Domain.Entities.UserEntities;

namespace ProjetoTelegram.Application.Interfaces.AuthInterfaces
{
    public interface IAuthService
    {
        public Task<Result<User>> Signup(AuthSignupModel signupModel);
        public Task<Result<string>> Login(AuthLoginModel authLoginModel);
        public string GenerateToken(ObjectId userId);
    }
}

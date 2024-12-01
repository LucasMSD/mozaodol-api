using FluentResults;
using MongoDB.Bson;

namespace ProjetoTelegram.Domain.Services
{
    public interface ITokenService
    {
        Result<string> GenerateToken(ObjectId userId, string userName);
    }
}

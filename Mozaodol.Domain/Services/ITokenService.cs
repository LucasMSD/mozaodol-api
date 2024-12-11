using FluentResults;
using MongoDB.Bson;

namespace Mozaodol.Domain.Services
{
    public interface ITokenService
    {
        Result<string> GenerateToken(ObjectId userId, string userName);
    }
}

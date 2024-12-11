using MongoDB.Bson;
using Mozaodol.Application.DTOs.UserDTOs;

namespace Mozaodol.Application.UseCases.UserUseCases
{
    public interface IGetUserDtoUseByIdUseCase : IUseCase<ObjectId, UserDto>
    {
    }
}

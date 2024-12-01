using FluentResults;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.UserDTOs;

namespace ProjetoTelegram.Application.UseCases.UserUseCases
{
    public interface IGetUserDtoUseByIdUseCase : IUseCase<ObjectId, Result<UserDto>>
    {
    }
}

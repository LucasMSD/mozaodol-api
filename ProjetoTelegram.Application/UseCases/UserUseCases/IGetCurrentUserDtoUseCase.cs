using FluentResults;
using ProjetoTelegram.Application.DTOs.UserDTOs;

namespace ProjetoTelegram.Application.UseCases.UserUseCases
{
    public interface IGetCurrentUserDtoUseCase : IUseCase<object?, Result<UserDto>>
    {
    }
}

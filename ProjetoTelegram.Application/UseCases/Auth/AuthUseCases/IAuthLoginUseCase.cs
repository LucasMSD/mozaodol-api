using FluentResults;
using ProjetoTelegram.Application.DTOs.AuthDTOs;

namespace ProjetoTelegram.Application.UseCases.Auth.AuthUseCases
{
    public interface IAuthLoginUseCase : IUseCase<AuthLoginModel, Result<string>>
    {
    }
}

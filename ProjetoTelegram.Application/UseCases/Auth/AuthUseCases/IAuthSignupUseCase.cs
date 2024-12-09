using ProjetoTelegram.Application.DTOs.AuthDTOs;
using ProjetoTelegram.Application.DTOs.UserDTOs;

namespace ProjetoTelegram.Application.UseCases.Auth.AuthUseCases
{
    public interface IAuthSignupUseCase : IUseCase<AuthSignupModel, UserDto>
    {
    }
}

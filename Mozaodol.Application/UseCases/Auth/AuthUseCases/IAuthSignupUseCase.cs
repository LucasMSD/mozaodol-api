using Mozaodol.Application.DTOs.AuthDTOs;
using Mozaodol.Application.DTOs.UserDTOs;

namespace Mozaodol.Application.UseCases.Auth.AuthUseCases
{
    public interface IAuthSignupUseCase : IUseCase<AuthSignupModel, UserDto>
    {
    }
}

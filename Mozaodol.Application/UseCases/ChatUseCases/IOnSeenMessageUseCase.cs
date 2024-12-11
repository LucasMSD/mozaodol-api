using Mozaodol.Application.DTOs.MessageDTOs;

namespace Mozaodol.Application.UseCases.ChatUseCases
{
    public interface IOnSeenMessageUseCase : IUseCase<SeenMessageDTO, object?>
    {
    }
}

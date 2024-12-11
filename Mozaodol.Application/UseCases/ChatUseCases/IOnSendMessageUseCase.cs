using Mozaodol.Application.DTOs.MessageDTOs;

namespace Mozaodol.Application.UseCases.ChatUseCases
{
    public interface IOnSendMessageUseCase : IUseCase<SendMessageDTO, MessageDto>
    {
    }
}

using FluentResults;
using ProjetoTelegram.Application.DTOs.MessageDTOs;

namespace ProjetoTelegram.Application.UseCases.ChatUseCases
{
    public interface IOnSendMessageUseCase : IUseCase<SendMessageDTO, Result<MessageDto>>
    {
    }
}

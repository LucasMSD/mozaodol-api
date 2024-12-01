using ProjetoTelegram.Application.DTOs.MessageDTOs;

namespace ProjetoTelegram.Application.UseCases.ChatUseCases
{
    public interface IOnSeenMessageUseCase : IUseCase<SeenMessageDTO, object?>
    {
    }
}

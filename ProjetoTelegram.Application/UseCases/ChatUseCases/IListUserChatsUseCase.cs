using FluentResults;
using ProjetoTelegram.Application.DTOs.ChatDTOs;

namespace ProjetoTelegram.Application.UseCases.ChatUseCases
{
    public interface IListUserChatsUseCase : IUseCase<object?, Result<IEnumerable<ChatDto>>>
    {
    }
}

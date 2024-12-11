using Mozaodol.Application.DTOs.ChatDTOs;

namespace Mozaodol.Application.UseCases.ChatUseCases
{
    public interface IListUserChatsUseCase : IUseCase<object?, IEnumerable<ChatDto>>
    {
    }
}

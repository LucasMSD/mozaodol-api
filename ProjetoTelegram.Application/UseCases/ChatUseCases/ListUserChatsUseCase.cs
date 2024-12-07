using FluentResults;
using ProjetoTelegram.Application.DTOs.ChatDTOs;
using ProjetoTelegram.Domain.Entities.ChatEntities;
using ProjetoTelegram.Domain.Repositories.ChatRepositories;
using ProjetoTelegram.Domain.Repositories.UserRepositories;

namespace ProjetoTelegram.Application.UseCases.ChatUseCases
{
    public class ListUserChatsUseCase :
        DefaultUseCase<object?, IEnumerable<ChatDto>>,
        IListUserChatsUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;

        public ListUserChatsUseCase(
            IUserRepository userRepository,
            IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public override async Task<Result<IEnumerable<ChatDto>>> Handle(object? input, CancellationToken cancellationToken)
        {
            // todo: refatorar
            var getUserResult = await _userRepository.Get(User.Id);
            if (getUserResult.IsFailed) return Result.Fail("Erro ao buscar usuário").WithErrors(getUserResult.Errors);

            var getChatResult = await _chatRepository.Get(getUserResult.Value.ChatsIds);
            if (getChatResult.IsFailed) return Result.Fail("Erro ao buscar chat").WithErrors(getChatResult.Errors);

            var getUsersResult = await _userRepository.Get(getChatResult.Value.SelectMany(x => x.UsersIds));
            if (getUsersResult.IsFailed) return Result.Fail("Erro ao buscar os usuários").WithErrors(getUsersResult.Errors);

            return getChatResult.Value.Select(chat => new ChatDto
            {
                _id = chat._id,
                Name = Chat.GenerateChatName(
                    getUsersResult.Value.Where(user => chat.UsersIds.Contains(user._id) && user._id != User.Id).ToArray())
            }).ToList();
        }
    }
}

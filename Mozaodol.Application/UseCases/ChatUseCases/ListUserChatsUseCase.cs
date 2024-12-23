using FluentResults;
using Mozaodol.Application.DTOs.ChatDTOs;
using Mozaodol.Application.Extensions.Results;
using Mozaodol.Domain.Entities.ChatEntities;
using Mozaodol.Domain.Repositories.ChatRepositories;
using Mozaodol.Domain.Repositories.UserRepositories;

namespace Mozaodol.Application.UseCases.ChatUseCases
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
            var user = await _userRepository.Get(User.Id);
            if (user == null) return Result.Fail("Usuário não existe").SetStatusCode(401);

            var chats = await _chatRepository.Get(user.ChatsIds);

            if (chats.Count == 0) return Enumerable.Empty<ChatDto>().ToResult(204);

            var chatUsers = await _userRepository.Get(chats.SelectMany(x => x.UsersIds));
            if (chatUsers.Count == 0) return Result.Fail("Usuários do chat não existem").SetStatusCode(500);

            return chats.Select(chat => new ChatDto
            {
                _id = chat._id,
                Name = Chat.GenerateChatName(
                    chatUsers.Where(user => chat.UsersIds.Contains(user._id) && user._id != User.Id).ToArray())
            }).ToResult(200);
        }
    }
}

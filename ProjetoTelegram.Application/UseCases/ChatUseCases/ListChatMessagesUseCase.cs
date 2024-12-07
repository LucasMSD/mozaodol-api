using FluentResults;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.MessageDTOs;
using ProjetoTelegram.Domain.Repositories.ChatRepositories;
using ProjetoTelegram.Domain.Repositories.MessageRepositories;
using ProjetoTelegram.Domain.Repositories.UserRepositories;

namespace ProjetoTelegram.Application.UseCases.ChatUseCases
{
    public class ListChatMessagesUseCase :
        DefaultUseCase<ObjectId, List<MessageDto>>,
        IListChatMessagesUseCase
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public ListChatMessagesUseCase(
            IChatRepository chatRepository,
            IUserRepository userRepository,
            IMessageRepository messageRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        public override async Task<Result<List<MessageDto>>> Handle(ObjectId chatId, CancellationToken cancellationToken)
        {
            var getChatResult = await _chatRepository.Get(chatId);
            if (getChatResult.IsFailed) return Result.Fail("Erro ao buscar o chat.").WithErrors(getChatResult.Errors);
            if (getChatResult.Value == null) return Result.Fail("Chat não encontrado.");

            var getUsersResult = await _userRepository.Get(getChatResult.Value.UsersIds);
            if (getUsersResult.IsFailed) return Result.Fail("Erro ao buscar usuários.").WithErrors(getUsersResult.Errors);
            if (!getUsersResult.Value.Any()) return Result.Fail("Usuários do chat não encontrados.");

            var getMessagesResult = await _messageRepository.GetByChat(chatId);
            if (getMessagesResult.IsFailed) return Result.Fail("Erro ao buscar mensagens.").WithErrors(getMessagesResult.Errors);
            if (!getMessagesResult.Value.Any()) return Result.Fail("Mensagens não encontradas.");

            var usersDict = getUsersResult.Value.ToDictionary(user => user._id);
            return getMessagesResult.Value.OrderByDescending(x => x.Timestamp).Select(message => new MessageDto
            {
                Text = message.Text,
                UserId = message.UserId,
                UserUsername = usersDict[message.UserId].Username,
                Timestamp = message.Timestamp.ToString("HH:mm"),
                ChatId = getChatResult.Value._id,
                Status = message.Status,
                ExternalId = string.IsNullOrEmpty(message.ExternalId) ? message._id.ToString() : message.ExternalId,
                _id = message._id
            }).ToList();
        }
    }
}

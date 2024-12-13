using FluentResults;
using MongoDB.Bson;
using Mozaodol.Application.DTOs.MessageDTOs;
using Mozaodol.Domain.Repositories.ChatRepositories;
using Mozaodol.Domain.Repositories.MessageRepositories;
using Mozaodol.Domain.Repositories.UserRepositories;

namespace Mozaodol.Application.UseCases.ChatUseCases
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
            var chat = await _chatRepository.Get(chatId);
            if (chat == null) return Result.Fail("Chat não encontrado.");

            var chatUsers = await _userRepository.Get(chat.UsersIds);
            if (chatUsers.Count == 0) return Result.Fail("Usuários do chat não encontrados.");

            var chatMessages = await _messageRepository.GetByChat(chatId);
            if (chatMessages.Count == 0) return Result.Fail("Mensagens não encontradas.");

            var usersDict = chatUsers.ToDictionary(user => user._id);
            return chatMessages.OrderByDescending(x => x.Timestamp).Select(message => new MessageDto
            {
                Text = message.Text,
                UserId = message.UserId,
                UserUsername = usersDict[message.UserId].Username,
                Timestamp = message.Timestamp.ToString("HH:mm"),
                ChatId = chat._id,
                Status = message.Status,
                ExternalId = string.IsNullOrEmpty(message.ExternalId) ? message._id.ToString() : message.ExternalId,
                _id = message._id
            }).ToList();
        }
    }
}

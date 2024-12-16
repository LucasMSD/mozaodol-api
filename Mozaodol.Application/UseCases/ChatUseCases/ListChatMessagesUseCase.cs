using FluentResults;
using MongoDB.Bson;
using Mozaodol.Application.DTOs.MessageDTOs;
using Mozaodol.Application.Extensions.Results;
using Mozaodol.Domain.Repositories.ChatRepositories;
using Mozaodol.Domain.Repositories.MessageRepositories;
using Mozaodol.Domain.Repositories.UserRepositories;
using Mozaodol.Domain.Services;

namespace Mozaodol.Application.UseCases.ChatUseCases
{
    public class ListChatMessagesUseCase :
        DefaultUseCase<ObjectId, List<MessageDto>>,
        IListChatMessagesUseCase
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IStorageService _storageService;

        public ListChatMessagesUseCase(
            IChatRepository chatRepository,
            IUserRepository userRepository,
            IMessageRepository messageRepository,
            IStorageService storageService)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _storageService = storageService;
        }

        public override async Task<Result<List<MessageDto>>> Handle(ObjectId chatId, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.Get(chatId);
            if (chat == null) return Result.Fail("Chat não encontrado.").SetStatusCode(404);

            var chatUsers = await _userRepository.Get(chat.UsersIds);
            if (chatUsers.Count == 0) return Result.Fail("Usuários do chat não encontrados.").SetStatusCode(500);

            var chatMessages = await _messageRepository.GetByChat(chatId);
            if (chatMessages.Count == 0) return Result.Ok().SetStatusCode(204);

            var usersDict = chatUsers.ToDictionary(user => user._id);

            var urlTasks = chatMessages.Where(x => x.Media != null).Select(x => new { id = x._id, UrlTask = _storageService.GetDownloadUrl(x.Media.StorageId, x.UserId)});
            await Task.WhenAll(urlTasks.Select(x => x.UrlTask));
            return chatMessages.OrderByDescending(x => x.Timestamp).Select(message => new MessageDto
            {
                Text = message.Text,
                UserId = message.UserId,
                UserUsername = usersDict[message.UserId].Username,
                Timestamp = message.Timestamp.ToString("HH:mm"),
                ChatId = chat._id,
                Status = message.Status,
                ExternalId = string.IsNullOrEmpty(message.ExternalId) ? message._id.ToString() : message.ExternalId,
                _id = message._id,
                Media = message.Media != null ? new ReceiveMessageMediaDto
                {
                    Url = urlTasks.First(x => x.id == message._id).UrlTask.Result.Value,
                    Type = message.Media.Type
                } : null
            }).ToList().ToResult(200);
        }
    }
}

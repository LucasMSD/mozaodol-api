using FluentResults;
using Microsoft.Extensions.Caching.Distributed;
using Mozaodol.Application.DTOs.MessageDTOs;
using Mozaodol.Application.DTOs.UserDTOs;
using Mozaodol.Domain.Entities.ChatEntities;
using Mozaodol.Domain.Entities.MessageEntities;
using Mozaodol.Domain.Enums;
using Mozaodol.Domain.Repositories.ChatRepositories;
using Mozaodol.Domain.Repositories.MessageRepositories;
using Mozaodol.Domain.Repositories.UserRepositories;
using Mozaodol.Domain.Services;
using System.Text.Json;

namespace Mozaodol.Application.UseCases.ChatUseCases
{
    public class OnSendMessageUseCase :
        DefaultUseCase<SendMessageDTO, MessageDto>,
        IOnSendMessageUseCase
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IRealTimeNotificationService _realTimeNotificationService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IDistributedCache _distributedCache;
        private readonly IStorageService _storageService;

        public OnSendMessageUseCase(
            IChatRepository chatRepository,
            IUserRepository userRepository,
            IMessageRepository messageRepository,
            IRealTimeNotificationService realTimeNotificationService,
            IPushNotificationService pushNotificationService,
            IDistributedCache distributedCache,
            IStorageService storageService)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _realTimeNotificationService = realTimeNotificationService;
            _pushNotificationService = pushNotificationService;
            _distributedCache = distributedCache;
            _storageService = storageService;
        }

        public override async Task<Result<MessageDto>> Handle(SendMessageDTO input, CancellationToken cancellationToken)
        {
            // validar se o chat existe
            // todo: se der algum erro, eu preciso avisar o client sobre o problema no envio
            var chat = await _chatRepository.Get(input.ChatId);
            if (chat == null) return Result.Fail("Chat não encontrado.");

            var user = await _userRepository.Get(User.Id);
            if (user == null) return Result.Fail("Usuário não encontrado.");

            // salvar mensagem
            var message = new Message
            {
                Text = input.Text,
                UserId = User.Id,
                ChatId = input.ChatId,
                Status = MessageStatus.Sent,
                Timestamp = DateTime.Now,
                ExternalId = input.ExternalId
            };

            if (input.Media != null)
            {
                var uploadResult = await _storageService.Upload(input.Media.ContentBase64, input.Media.Type, user._id);
                if (uploadResult.IsFailed)
                    return Result.Fail(uploadResult.Errors);

                message.Media = new MessageMedia()
                {
                    StorageId = uploadResult.Value,
                    Type = input.Media.Type,
                };
            }

            await _messageRepository.Insert(message);

            var messageDto = new MessageDto()
            {
                _id = message._id,
                UserId = message.UserId,
                ChatId = message.ChatId,
                Text = message.Text,
                UserUsername = user.Username,
                Status = message.Status,
                ExternalId = message.ExternalId,
                Timestamp = message.Timestamp.ToString("HH:mm"),
            };

            if (message.Media != null)
            {
                messageDto.Media = new ReceiveMessageMediaDto
                {
                    DownloadUrl = await _storageService.GetDownloadUrl(message.Media.StorageId),
                    Type = message.Media.Type
                };
            }

            var usersToSendNotification = chat.UsersIds.Where(x => x != message.UserId).Select(x => x.ToString());

            await _realTimeNotificationService.Notify(usersToSendNotification, new RealTimeNotificationMessage
            {
                ChannelId = "ReceiveMessage",
                Content = messageDto
            });
            await _realTimeNotificationService.Notify(User.Id.ToString(), new RealTimeNotificationMessage
            {
                ChannelId = $"MessageStatusUpdate-{input.ExternalId}",
                Content = MessageStatus.Sent
            });

            await SendPushNotifications(messageDto, chat);

            return messageDto;
        }

        public async Task<Result> SendPushNotifications(MessageDto message, Chat chat)
        {
            // todo: fazer mais validações
            var chatUsersIds = chat.UsersIds.Where(x => x != message.UserId);
            var users = new List<UserState>();

            // todo: refatorar para não depender apenas do cache (?)
            foreach (var userId in chatUsersIds)
            {
                var userJson = await _distributedCache.GetStringAsync(userId.ToString());
                if (string.IsNullOrEmpty(userJson)) continue;
                users.Add(JsonSerializer.Deserialize<UserState>(userJson));
            }

            var usersToPush = users.Where(x => !string.IsNullOrEmpty(x.PushToken) && (!x.Connected || x.Connected && x.OpenedChatId != chat._id.ToString()));

            await _pushNotificationService.Notify(usersToPush.Select(x => x.PushToken), new PushNotificationMessage
            {
                ChannelId = "receivedMessage",
                Content = message.Text,
                Priority = "high",
                Title = message.UserUsername,
                SubTitle = "Ainda não entendi onde aparece essa mensagem"
            });

            return Result.Ok();
        }
    }
}

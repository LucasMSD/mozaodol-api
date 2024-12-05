using FluentResults;
using Microsoft.Extensions.Caching.Distributed;
using ProjetoTelegram.Application.DTOs.MessageDTOs;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using ProjetoTelegram.Domain.Entities.ChatEntities;
using ProjetoTelegram.Domain.Entities.MessageEntities;
using ProjetoTelegram.Domain.Enums;
using ProjetoTelegram.Domain.Repositories.ChatRepositories;
using ProjetoTelegram.Domain.Repositories.MessageRepositories;
using ProjetoTelegram.Domain.Repositories.UserRepositories;
using ProjetoTelegram.Domain.Services;
using System.Text.Json;

namespace ProjetoTelegram.Application.UseCases.ChatUseCases
{
    public class OnSendMessageUseCase :
        DefaultUseCase<SendMessageDTO, Result<MessageDto>>,
        IOnSendMessageUseCase
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IRealTimeNotificationService _realTimeNotificationService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IDistributedCache _distributedCache;

        public OnSendMessageUseCase(
            IChatRepository chatRepository,
            IUserRepository userRepository,
            IMessageRepository messageRepository,
            IRealTimeNotificationService realTimeNotificationService,
            IPushNotificationService pushNotificationService,
            IDistributedCache distributedCache)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _realTimeNotificationService = realTimeNotificationService;
            _pushNotificationService = pushNotificationService;
            _distributedCache = distributedCache;
        }

        public override async Task<Result<MessageDto>> Handle(SendMessageDTO input, CancellationToken cancellationToken)
        {
            // validar se o chat existe
            // todo: se der algum erro, eu preciso avisar o client sobre o problema no envio
            var getChatResult = await _chatRepository.Get(input.ChatId);
            if (getChatResult.IsFailed) return Result.Fail("Erro ao buscar chat.").WithErrors(getChatResult.Errors);
            if (getChatResult.Value == null) return Result.Fail("Chat não encontrado.");

            var getUserResult = await _userRepository.Get(User.Id);
            if (getUserResult.IsFailed) return Result.Fail("Erro ao buscar usuário.").WithErrors(getUserResult.Errors);
            if (getUserResult.Value == null) return Result.Fail("Usuário não encontrado.");

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

            await _messageRepository.Insert(message);

            var messageDto = new MessageDto()
            {
                _id = message._id,
                UserId = message.UserId,
                ChatId = message.ChatId,
                Text = message.Text,
                UserUsername = getUserResult.Value.Username,
                Status = message.Status,
                ExternalId = message.ExternalId,
                Timestamp = message.Timestamp.ToString("HH:mm"),
            };

            var usersToSendNotification = getChatResult.Value.UsersIds.Where(x => x != message.UserId).Select(x => x.ToString());

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

            await SendPushNotifications(messageDto, getChatResult.Value);

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

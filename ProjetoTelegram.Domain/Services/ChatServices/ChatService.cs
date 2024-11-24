using Expo.Server.Client;
using Expo.Server.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using ProjetoTelegram.Domain.Hubs;
using ProjetoTelegram.Domain.Models.Chat;
using ProjetoTelegram.Domain.Models.Chat.Message;
using ProjetoTelegram.Domain.Models.User;
using ProjetoTelegram.Domain.Repositories.ChatRepositories;
using ProjetoTelegram.Domain.Repositories.MessageRepositories;
using ProjetoTelegram.Domain.Repositories.UserRepositories;
using System.Text.Json;

namespace ProjetoTelegram.Domain.Services.ChatServices
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IHubContext<ChatHub> _chatHub;

        public ChatService(
            IChatRepository chatRepository,
            IMessageRepository messageRepository,
            IUserRepository userRepository,
            IDistributedCache distributedCache,
            IHubContext<ChatHub> chatHub)
        {
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _distributedCache = distributedCache;
            _chatHub = chatHub;
        }

        public async Task<ObjectId> CreateChat(CreateChatModel chatModel)
        {
            var chat = new Chat
            {
                UsersIds = chatModel.UsersIds,
            };

            await _chatRepository.Insert(chat);

            await _userRepository.AddToChat(chat.UsersIds, chat._id);

            return chat._id;
        }

        public async Task<IEnumerable<ChatDto>> GetAll(ObjectId userId)
        {
            // todo: refatorar
            var user = await _userRepository.Get(userId);


            var chats = await _chatRepository.Get(user.ChatsIds);

            return chats.Select(x => new ChatDto
            {
                _id = x._id,
                Name = CreateChatName(x.UsersIds.Where(_id => _id != userId)).Result
            });
        }

        private async Task<string> CreateChatName(IEnumerable<ObjectId> usersIds)
        {
            var name = "";

            var users = (await _userRepository.Get(usersIds)).ToArray();

            for (var i = 0; i < users.Length; i++)
            {
                if (i != 0)
                {
                    if (i == users.Length - 1)
                        name += " e ";
                    else
                        name += ", ";
                }

                name += users[i].Username;
            }

            return name;
        }

        public async Task<MessageDto> SendMessage(NewMessageModel newMessage)
        {
            // validar se o chat existe
            var chat = await _chatRepository.Get(newMessage.ChatId);

            if (chat == null)
                return null;

            var user = await _userRepository.Get(newMessage.UserId);

            // salvar mensagem
            var message = new Message
            {
                Text = newMessage.Text,
                UserId = newMessage.UserId,
                ChatId = newMessage.ChatId,
                Status = Enums.MessageStatus.Sent,
                Timestamp = DateTime.Now,
                ExternalId = newMessage.ExternalId
            };

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

            var usersIds = chat.UsersIds.Where(x => x != message.UserId).Select(x => x.ToString());
            await _chatHub.Clients.Users(usersIds).SendAsync("ReceiveMessage", messageDto);
            await _chatHub.Clients.User(user._id.ToString()).SendAsync($"MessageStatusUpdate-{newMessage.ExternalId}", Enums.MessageStatus.Sent);

            await SendNotifications(messageDto, chat);

            return messageDto;
        }

        public async Task<IEnumerable<MessageDto>> GetMessages(ObjectId currentUserId, ObjectId chatId)
        {
            var chat = await _chatRepository.Get(chatId);
            var users = (await _userRepository.Get(chat.UsersIds)).ToDictionary(x => x._id);
            var messages = await _messageRepository.GetByChat(chatId);

            return messages.OrderByDescending(x => x.Timestamp).Select(x => new MessageDto
            {
                Text = x.Text,
                UserId = x.UserId,
                UserUsername = users[x.UserId].Username,
                Timestamp = x.Timestamp.ToString("HH:mm"),
                ChatId = chat._id,
                Status = x.Status,
                ExternalId = string.IsNullOrEmpty(x.ExternalId) ? x._id.ToString() : x.ExternalId,
                _id = x._id
            });
        }

        public async Task SendNotifications(MessageDto message, Chat chat)
        {
            var chatUsersIds = chat.UsersIds.Where(x => x != message.UserId);
            var users = new List<UserState>();

            foreach (var userId in chatUsersIds)
            {
                var userJson = await _distributedCache.GetStringAsync(userId.ToString());
                if (string.IsNullOrEmpty(userJson)) continue;
                users.Add(JsonSerializer.Deserialize<UserState>(userJson));
            }

            var usersToPush = users.Where(x => !string.IsNullOrEmpty(x.PushToken) && (!x.Connected || (x.Connected && x.OpenedChatId != chat._id.ToString())));

            var expoSDKClient = new PushApiClient();
            var pushTicketReq = new PushTicketRequest()
            {
                PushTo = usersToPush.Select(x => x.PushToken).ToList(),
                PushBody = message.Text,
                PushTitle = message.UserUsername,
                PushChannelId = "receivedMessage",
                PushSubTitle = "Teste para ver onde aparece isso",
                PushPriority = "high"
            };
            var result = await expoSDKClient.PushSendAsync(pushTicketReq);
        }

        public async Task SeenMessage(SeenMessageModel seenMessage)
        {
            var message = await _messageRepository.Get(seenMessage.MessageId);

            await _messageRepository.UpdateStatus(message._id, Enums.MessageStatus.Seen);
            await _chatHub.Clients.User(message.UserId.ToString()).SendAsync($"MessageStatusUpdate-{message.ExternalId}", Enums.MessageStatus.Seen);
        }
    }
}

using Expo.Server.Client;
using Expo.Server.Models;
using FluentResults;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.ChatDTOs;
using ProjetoTelegram.Application.DTOs.MessageDTOs;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using ProjetoTelegram.Application.Interfaces.ChatInterfaces;
using ProjetoTelegram.Application.Interfaces.UserInterfaces;
using ProjetoTelegram.Domain.Entities.ChatEntities;
using ProjetoTelegram.Domain.Entities.MessageEntities;
using ProjetoTelegram.Domain.Entities.UserEntities;
using ProjetoTelegram.Domain.Enums;
using ProjetoTelegram.Domain.Repositories.ChatRepositories;
using ProjetoTelegram.Domain.Repositories.MessageRepositories;
using ProjetoTelegram.Domain.Repositories.UserRepositories;
using System.Text.Json;

namespace ProjetoTelegram.Application.Implementations.ChatImplementations
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMessageRepository _messageRepository;
        private readonly IDistributedCache _distributedCache;

        public ChatService(
            IChatRepository chatRepository,
            IMessageRepository messageRepository,
            IUserRepository userRepository,
            IDistributedCache distributedCache,
            IUserService userService)
        {
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _distributedCache = distributedCache;
            _userService = userService;
        }

        public async Task<Result<ObjectId>> CreateChat(CreateChatModel chatModel)
        {
            var chat = new Chat
            {
                UsersIds = chatModel.UsersIds,
            };

            var insertResult = await _chatRepository.Insert(chat);
            if (insertResult.IsFailed) return Result.Fail("Erro ao criar chat.").WithErrors(insertResult.Errors);


            var addUserToChatResult =  await _userRepository.AddToChat(chat.UsersIds, chat._id);
            if (addUserToChatResult.IsFailed) return Result.Fail("Erro ao incluir usuários no chat.").WithErrors(addUserToChatResult.Errors);

            return chat._id;
        }

        public async Task<Result<List<ChatDto>>> GetAll(ObjectId userId)
        {
            // todo: refatorar
            var getUserResult = await _userRepository.Get(userId);
            if (getUserResult.IsFailed) return Result.Fail("Erro ao buscar usuário").WithErrors(getUserResult.Errors);

            var getChatResult = await _chatRepository.Get(getUserResult.Value.ChatsIds);
            if (getChatResult.IsFailed) return Result.Fail("Erro ao buscar chat").WithErrors(getChatResult.Errors);

            var getUsersResult = await _userRepository.Get(getChatResult.Value.SelectMany(x => x.UsersIds));
            if (getUsersResult.IsFailed) return Result.Fail("Erro ao buscar os usuários").WithErrors(getUsersResult.Errors);

            return getChatResult.Value.Select(chat => new ChatDto
            {
                _id = chat._id,
                Name = GenerateChatName(getUsersResult.Value.Where(user => chat.UsersIds.Contains(user._id)).ToArray())
            }).ToList();
        }

        private string GenerateChatName(User[] users)
        {
            var name = "";

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

        public async Task<Result<(MessageDto, List<string>)>> SendMessage(NewMessageModel newMessage)
        {
            // validar se o chat existe
            var getChatResult = await _chatRepository.Get(newMessage.ChatId);
            if (getChatResult.IsFailed) return Result.Fail("Erro ao buscar chat.").WithErrors(getChatResult.Errors);
            if (getChatResult.Value == null) return Result.Fail("Chat não encontrado.");

            var getUserResult = await _userRepository.Get(newMessage.UserId);
            if (getUserResult.IsFailed) return Result.Fail("Erro ao buscar usuário.").WithErrors(getUserResult.Errors);
            if (getUserResult.Value == null) return Result.Fail("Usuário não encontrado.");

            // salvar mensagem
            var message = new Message
            {
                Text = newMessage.Text,
                UserId = newMessage.UserId,
                ChatId = newMessage.ChatId,
                Status = MessageStatus.Sent,
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
                UserUsername = getUserResult.Value.Username,
                Status = message.Status,
                ExternalId = message.ExternalId,
                Timestamp = message.Timestamp.ToString("HH:mm"),
            };

            var usersIds = getChatResult.Value.UsersIds.Where(x => x != message.UserId).Select(x => x.ToString());


            await SendNotifications(messageDto, getChatResult.Value);

            return (messageDto, usersIds.ToList());
        }

        public async Task<Result<List<MessageDto>>> GetMessages(ObjectId currentUserId, ObjectId chatId)
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

        public async Task<Result> SendNotifications(MessageDto message, Chat chat)
        {
            // todo: fazer mais validações
            var chatUsersIds = chat.UsersIds.Where(x => x != message.UserId);
            var users = new List<UserState>();

            foreach (var userId in chatUsersIds)
            {
                var userJson = await _distributedCache.GetStringAsync(userId.ToString());
                if (string.IsNullOrEmpty(userJson)) continue;
                users.Add(JsonSerializer.Deserialize<UserState>(userJson));
            }

            var usersToPush = users.Where(x => !string.IsNullOrEmpty(x.PushToken) && (!x.Connected || x.Connected && x.OpenedChatId != chat._id.ToString()));

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

            return Result.Ok();
        }

        public async Task<Result<Message>> SeenMessage(SeenMessageModel seenMessage)
        {
            var getMessageResult = await _messageRepository.Get(seenMessage.MessageId);
            if (getMessageResult.IsFailed) return Result.Fail("Erro ao buscar a mensagem.").WithErrors(getMessageResult.Errors);

            await _messageRepository.UpdateStatus(getMessageResult.Value._id, MessageStatus.Seen);
            return getMessageResult.Value;
        }

        public async Task OnOpenedChat(OnOpenedChatModel onOpenedChatModel, string userIdentifier)
        {
            var userStateJson = await _distributedCache.GetStringAsync(userIdentifier);
            var userState = JsonSerializer.Deserialize<UserState>(userStateJson);

            userState.Connected = true;
            userState.OpenedChatId = onOpenedChatModel.ChatId.ToString();

            await _distributedCache.RemoveAsync(userIdentifier);
            await _distributedCache.SetStringAsync(userIdentifier, JsonSerializer.Serialize(userState));
        }

        public async Task OnLeftChat(string userIdentifier)
        {
            var userStateJson = await _distributedCache.GetStringAsync(userIdentifier);
            var userState = JsonSerializer.Deserialize<UserState>(userStateJson);

            userState.Connected = true;
            userState.OpenedChatId = null;

            await _distributedCache.RemoveAsync(userIdentifier);
            await _distributedCache.SetStringAsync(userIdentifier, JsonSerializer.Serialize(userState));
        }

        public async Task OnDisconnected(string userId, Exception? exception)
        {
            var userStateJson = await _distributedCache.GetStringAsync(userId);

            if (string.IsNullOrEmpty(userStateJson)) return;

            var userState = JsonSerializer.Deserialize<UserState>(userStateJson);
            userState.Connected = false;

            await _distributedCache.RemoveAsync(userId);
            await _distributedCache.SetStringAsync(userId, JsonSerializer.Serialize(userState));
        }

        public async Task OnConnected(string userId)
        {
            var userSateJson = await _distributedCache.GetStringAsync(userId);

            if (string.IsNullOrEmpty(userSateJson))
            {
                var getUserResult = await _userService.Get(new ObjectId(userId));
                if (getUserResult.IsFailed) return;

                userSateJson = JsonSerializer.Serialize(new UserState()
                {
                    UserId = getUserResult.Value._id.ToString(),
                    PushToken = getUserResult.Value.PushToken,
                    Connected = true
                });

                await _distributedCache.SetStringAsync(userId, userSateJson);
                return;
            }

            var userState = JsonSerializer.Deserialize<UserState>(userSateJson);
            userState.Connected = true;

            await _distributedCache.RemoveAsync(userId);
            await _distributedCache.SetStringAsync(userId, JsonSerializer.Serialize(userState));
        }
    }
}

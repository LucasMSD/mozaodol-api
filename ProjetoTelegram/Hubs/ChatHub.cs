using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using ProjetoTelegram.Domain.Enums;
using ProjetoTelegram.Domain.Models.Chat;
using ProjetoTelegram.Domain.Models.Chat.Message;
using ProjetoTelegram.Domain.Models.User;
using ProjetoTelegram.Domain.Services.ChatServices;
using ProjetoTelegram.Domain.Services.UserServices;
using System.Text.Json;

namespace ProjetoTelegram.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IUserService _userService;
        private readonly IDistributedCache _distributedCache;
        private readonly IChatService _chatService;
        public ChatHub(IUserService userService, IDistributedCache distributedCache, IChatService chatService)
        {
            _userService = userService;
            _distributedCache = distributedCache;
            _chatService = chatService;
        }

        public async Task OnOpenedChat(OnOpenedChatModel onOpenedChatModel)
        {
            var userStateJson = await _distributedCache.GetStringAsync(Context.UserIdentifier);
            var userState = JsonSerializer.Deserialize<UserState>(userStateJson);

            userState.Connected = true;
            userState.OpenedChatId = onOpenedChatModel.ChatId.ToString();

            await _distributedCache.RemoveAsync(Context.UserIdentifier);
            await _distributedCache.SetStringAsync(Context.UserIdentifier, JsonSerializer.Serialize(userState));
        }

        public async Task OnLeftChat()
        {
            var userStateJson = await _distributedCache.GetStringAsync(Context.UserIdentifier);
            var userState = JsonSerializer.Deserialize<UserState>(userStateJson);

            userState.Connected = true;
            userState.OpenedChatId = null;

            await _distributedCache.RemoveAsync(Context.UserIdentifier);
            await _distributedCache.SetStringAsync(Context.UserIdentifier, JsonSerializer.Serialize(userState));
        }

        public async Task CreateChat(CreateChatModel chatModel)
        {
            var chatId = await _chatService.CreateChat(chatModel);
            await Clients.User(Context.UserIdentifier).SendAsync("ChatCreated", new { ChatId = chatId });
        }

        public async Task OnSendMessage(NewMessageModel newMessage)
        {
            newMessage.UserId = new ObjectId(Context.UserIdentifier);
            (MessageDto sendMessage, IEnumerable<string> idsToSend) = await _chatService.SendMessage(newMessage);
            await Clients.Users(idsToSend).SendAsync("ReceiveMessage", sendMessage);
            await Clients.User(sendMessage.UserId.ToString()).SendAsync($"MessageStatusUpdate-{newMessage.ExternalId}", MessageStatus.Sent);
        }
        public async Task OnSeenMessage(SeenMessageModel seenMessage)
        {
            // todo: reforar esse código
            var message = await _chatService.SeenMessage(seenMessage);
            await Clients.User(message.UserId.ToString()).SendAsync($"MessageStatusUpdate-{message.ExternalId}", MessageStatus.Seen);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userStateJson = await _distributedCache.GetStringAsync(Context.UserIdentifier);

            if (string.IsNullOrEmpty(userStateJson)) return;

            var userState = JsonSerializer.Deserialize<UserState>(userStateJson);
            userState.Connected = false;

            await _distributedCache.RemoveAsync(Context.UserIdentifier);
            await _distributedCache.SetStringAsync(Context.UserIdentifier, JsonSerializer.Serialize(userState));
        }

        public override async Task OnConnectedAsync()
        {
            var userSateJson = await _distributedCache.GetStringAsync(Context.UserIdentifier);

            if (string.IsNullOrEmpty(userSateJson))
            {
                var user = await _userService.Get(new ObjectId(Context.UserIdentifier));

                userSateJson = JsonSerializer.Serialize(new UserState()
                {
                    UserId = user._id.ToString(),
                    PushToken = user.PushToken,
                    Connected = true
                });

                await _distributedCache.SetStringAsync(Context.UserIdentifier, userSateJson);
                return;
            }

            var userState = JsonSerializer.Deserialize<UserState>(userSateJson);
            userState.Connected = true;

            await _distributedCache.RemoveAsync(Context.UserIdentifier);
            await _distributedCache.SetStringAsync(Context.UserIdentifier, JsonSerializer.Serialize(userState));
        }
    }
}

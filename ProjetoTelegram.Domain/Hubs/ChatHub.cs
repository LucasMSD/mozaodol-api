using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using ProjetoTelegram.Domain.Models.Chat;
using ProjetoTelegram.Domain.Models.User;
using ProjetoTelegram.Domain.Repositories;
using ProjetoTelegram.Domain.Services.ChatServices;
using ProjetoTelegram.Domain.Services.UserServices;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace ProjetoTelegram.Domain.Hubs
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

        public async Task<bool> Teste()
        {
            return true;
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
            var sendMessage = await _chatService.SendMessage(newMessage);
        }
        public async Task OnSeenMessage(SeenMessageModel seenMessage)
        {
            await _chatService.SeenMessage(seenMessage);
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

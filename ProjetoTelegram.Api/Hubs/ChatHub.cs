using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.ChatDTOs;
using ProjetoTelegram.Application.DTOs.MessageDTOs;
using ProjetoTelegram.Application.Interfaces.ChatInterfaces;
using ProjetoTelegram.Application.Interfaces.UserInterfaces;
using ProjetoTelegram.Domain.Enums;

namespace ProjetoTelegram.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task OnOpenedChat(OnOpenedChatModel onOpenedChatModel)
            => await _chatService.OnOpenedChat(onOpenedChatModel, Context.UserIdentifier);


        public async Task OnLeftChat()
            => await _chatService.OnLeftChat(Context.UserIdentifier);

        public async Task CreateChat(CreateChatModel chatModel)
            => await _chatService.CreateChat(chatModel, Context.UserIdentifier);

        public async Task OnSendMessage(NewMessageModel newMessage)
        {
            newMessage.UserId = new ObjectId(Context.UserIdentifier);
            await _chatService.SendMessage(newMessage);
        }
        public async Task OnSeenMessage(SeenMessageModel seenMessage)
            => await _chatService.SeenMessage(seenMessage, Context.UserIdentifier);

        
        public override async Task OnDisconnectedAsync(Exception? exception)
            => await _chatService.OnDisconnected(Context.UserIdentifier, exception);

        public override async Task OnConnectedAsync()
            => await _chatService.OnConnected(Context.UserIdentifier);
    }
}

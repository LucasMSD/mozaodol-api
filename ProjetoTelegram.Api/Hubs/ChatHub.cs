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
        {
            var chatId = await _chatService.CreateChat(chatModel);
            await Clients.User(Context.UserIdentifier).SendAsync("ChatCreated", new { ChatId = chatId });
        }

        public async Task OnSendMessage(NewMessageModel newMessage)
        {
            newMessage.UserId = new ObjectId(Context.UserIdentifier);
            Result<(MessageDto messageDto, List<string> userIds)> sendMessageResult = await _chatService.SendMessage(newMessage);

            if (sendMessageResult.IsFailed) return;

            await Clients.Users(sendMessageResult.Value.userIds).SendAsync("ReceiveMessage", sendMessageResult.Value.messageDto);
            await Clients.User(sendMessageResult.Value.messageDto.UserId.ToString()).SendAsync($"MessageStatusUpdate-{newMessage.ExternalId}", MessageStatus.Sent);
        }
        public async Task OnSeenMessage(SeenMessageModel seenMessage)
        {
            // todo: reforar esse código
            var messageResult = await _chatService.SeenMessage(seenMessage);

            if (messageResult.IsFailed) return;
            await Clients.User(messageResult.Value.UserId.ToString()).SendAsync($"MessageStatusUpdate-{messageResult.Value.ExternalId}", MessageStatus.Seen);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
            => await _chatService.OnDisconnected(Context.UserIdentifier, exception);

        public override async Task OnConnectedAsync()
            => await _chatService.OnConnected(Context.UserIdentifier);
    }
}

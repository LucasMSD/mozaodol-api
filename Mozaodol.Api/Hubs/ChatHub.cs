using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozaodol.Application.DTOs.ChatDTOs;
using Mozaodol.Application.DTOs.MessageDTOs;
using Mozaodol.Application.UseCases.ChatUseCases;

namespace Mozaodol.Api.Hubs
{
    [Authorize]
    public class ChatHub : BaseHub
    {
        private readonly IOnDisconnectedUseCase _onDisconnectedUseCase;
        private readonly IOnConnectedUseCase _onConnectedUseCase;

        public ChatHub(
            IOnDisconnectedUseCase onDisconnectedUseCase,
            IOnConnectedUseCase onConnectedUseCase)
        {
            _onDisconnectedUseCase = onDisconnectedUseCase;
            _onConnectedUseCase = onConnectedUseCase;
        }

        public override async Task OnDisconnectedAsync(
            Exception? exception)
            => await RunAsync(_onDisconnectedUseCase, exception);

        public override async Task OnConnectedAsync()
            => await RunAsync(_onConnectedUseCase, null);

        public async Task OnOpenedChat(
            OnOpenedChatDTO input,
            [FromServices] IOnOpenedChatUseCase onOpenedChatUseCase)
            => await RunAsync(onOpenedChatUseCase, input);

        public async Task OnTyping(
            OnTypingDTO input,
            [FromServices] IOnTypingUseCase onTypingUseCase)
            => await RunAsync(onTypingUseCase, input);


        public async Task OnLeftChat(
            [FromServices] IOnLeftChatUseCase onLeftChatUseCase)
            => await RunAsync(onLeftChatUseCase, null);

        public async Task OnSendMessage(
            SendMessageDTO input,
            
            [FromServices] IOnSendMessageUseCase onSendMessageUseCase)
            => await RunAsync(onSendMessageUseCase, input);

        public async Task OnSeenMessage(
            SeenMessageDTO input,
            [FromServices] IOnSeenMessageUseCase onSeenMessageUseCase)
            => await RunAsync(onSeenMessageUseCase, input);
    }
}

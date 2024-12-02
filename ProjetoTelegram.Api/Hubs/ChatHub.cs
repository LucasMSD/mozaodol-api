using Microsoft.AspNetCore.Authorization;
using ProjetoTelegram.Application.DTOs.ChatDTOs;
using ProjetoTelegram.Application.DTOs.MessageDTOs;
using ProjetoTelegram.Application.UseCases.ChatUseCases;

namespace ProjetoTelegram.Api.Hubs
{
    [Authorize]
    public class ChatHub : BaseHub
    {
        private readonly IOnOpenedChatUseCase _onOpenedChatUseCase;
        private readonly IOnLeftChatUseCase _onLeftChatUseCase;
        private readonly IOnSendMessageUseCase _onSendMessageUseCase;
        private readonly IOnSeenMessageUseCase _onSeenMessageUseCase;
        private readonly IOnDisconnectedUseCase _onDisconnectedUseCase;
        private readonly IOnConnectedUseCase _onConnectedUseCase;
        private readonly IOnTypingUseCase _onTypingUseCase;

        public ChatHub(
            IOnOpenedChatUseCase onOpenedChatUseCase,
            IOnLeftChatUseCase onLeftChatUseCase,
            IOnSendMessageUseCase onSendMessageUseCase,
            IOnSeenMessageUseCase onSeenMessageUseCase,
            IOnDisconnectedUseCase onDisconnectedUseCase,
            IOnConnectedUseCase onConnectedUseCase,
            IOnTypingUseCase onTypingUseCase)
        {
            _onOpenedChatUseCase = onOpenedChatUseCase;
            _onLeftChatUseCase = onLeftChatUseCase;
            _onSendMessageUseCase = onSendMessageUseCase;
            _onSeenMessageUseCase = onSeenMessageUseCase;
            _onDisconnectedUseCase = onDisconnectedUseCase;
            _onConnectedUseCase = onConnectedUseCase;
            _onTypingUseCase = onTypingUseCase;
        }

        public async Task OnOpenedChat(OnOpenedChatDTO input)
            => await RunAsync(_onOpenedChatUseCase, input);


        public async Task OnTyping(
            OnTypingDTO input)
            => await RunAsync(_onTypingUseCase, input);


        public async Task OnLeftChat()
            => await RunAsync(_onLeftChatUseCase, null);

        public async Task OnSendMessage(
            SendMessageDTO input)
            => await RunAsync(_onSendMessageUseCase, input);


        public async Task OnSeenMessage(
            SeenMessageDTO input)
            => await RunAsync(_onSeenMessageUseCase, input);

        
        public override async Task OnDisconnectedAsync(
            Exception? exception)
            => await RunAsync(_onDisconnectedUseCase, exception);

        public override async Task OnConnectedAsync()
            => await RunAsync(_onConnectedUseCase, null);
    }
}

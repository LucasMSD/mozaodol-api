
using FluentResults;
using ProjetoTelegram.Application.DTOs.ChatDTOs;
using ProjetoTelegram.Domain.Repositories.ChatRepositories;
using ProjetoTelegram.Domain.Services;

namespace ProjetoTelegram.Application.UseCases.ChatUseCases
{
    public class OnTypingUseCase :
        DefaultUseCase<OnTypingDTO, object?>,
        IOnTypingUseCase
    {
        private readonly IChatRepository _chatRepository;
        private readonly IRealTimeNotificationService _realTimeNotificationService;

        public OnTypingUseCase(
            IChatRepository chatRepository,
            IRealTimeNotificationService realTimeNotificationService)
        {
            _chatRepository = chatRepository;
            _realTimeNotificationService = realTimeNotificationService;
        }

        public override async Task<Result<object?>> Handle(OnTypingDTO input, CancellationToken cancellationToken)
        {
            await _realTimeNotificationService.NotifyGroupExcept(input.ChatId.ToString(), User.Connection, new RealTimeNotificationMessage
            {
                ChannelId = "UserTypingStatus",
                Content = input.IsTyping,
            });

            return null;
        }
    }
}

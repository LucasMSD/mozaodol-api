
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
        private readonly INotificationService<IRealTimeNotificationMessage> _realTimeNotificationService;

        public OnTypingUseCase(
            IChatRepository chatRepository,
            INotificationService<IRealTimeNotificationMessage> realTimeNotificationService)
        {
            _chatRepository = chatRepository;
            _realTimeNotificationService = realTimeNotificationService;
        }

        public override async Task<object?> Handle(OnTypingDTO input, CancellationToken cancellationToken)
        {
            var chatResult = await _chatRepository.Get(input.ChatId);

            var usersToNotify = chatResult.Value.UsersIds.Where(userId => userId != User.Id).Select(x => x.ToString());
            await _realTimeNotificationService.Notify(usersToNotify, new RealTimeNotificationMessage
            {
                ChannelId = "UserTypingStatus",
            });

            return null;
        }
    }
}

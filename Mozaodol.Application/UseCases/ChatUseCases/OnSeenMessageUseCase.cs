using FluentResults;
using Mozaodol.Application.DTOs.MessageDTOs;
using Mozaodol.Domain.Enums;
using Mozaodol.Domain.Repositories.MessageRepositories;
using Mozaodol.Domain.Services;

namespace Mozaodol.Application.UseCases.ChatUseCases
{
    public class OnSeenMessageUseCase :
        DefaultUseCase<SeenMessageDTO, object?>,
        IOnSeenMessageUseCase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IRealTimeNotificationService _realTimeNotificationService;

        public OnSeenMessageUseCase(
            IMessageRepository messageRepository,
            IRealTimeNotificationService realTimeNotificationService)
        {
            _messageRepository = messageRepository;
            _realTimeNotificationService = realTimeNotificationService;
        }

        public override async Task<Result<object?>> Handle(SeenMessageDTO input, CancellationToken cancellationToken)
        {
            var getMessageResult = await _messageRepository.Get(input.MessageId);
            if (getMessageResult.IsFailed) return Result.Fail("Erro ao buscar a mensagem.").WithErrors(getMessageResult.Errors);
            await _messageRepository.UpdateStatus(getMessageResult.Value._id, MessageStatus.Seen);

            await _realTimeNotificationService.Notify([getMessageResult.Value.UserId.ToString()], new RealTimeNotificationMessage
            {
                ChannelId = $"MessageStatusUpdate-{getMessageResult.Value.ExternalId}",
                Content = MessageStatus.Seen
            });
            return getMessageResult.Value;
        }
    }
}

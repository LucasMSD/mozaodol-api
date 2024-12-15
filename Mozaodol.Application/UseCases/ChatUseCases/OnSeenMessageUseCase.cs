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
            var message = await _messageRepository.Get(input.MessageId);
            if (message == null) return Result.Fail("Mensagem não existe");
            await _messageRepository.UpdateStatus(message._id, MessageStatus.Seen);

            await _realTimeNotificationService.Notify([message.UserId.ToString()], new RealTimeNotificationMessage
            {
                ChannelId = $"MessageStatusUpdate-{message.ExternalId}",
                Content = MessageStatus.Seen
            });
            return Result.Ok();
        }
    }
}

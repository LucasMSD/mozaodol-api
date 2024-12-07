using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjetoTelegram.Application.UseCases.ChatUseCases;

namespace ProjetoTelegram.Api.Controllers.Chat
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ChatController : BaseController
    {

        [HttpGet]
        public async Task<IActionResult> List(
            [FromServices] IListUserChatsUseCase useCase,
            CancellationToken cancellationToken)
            => await RunAsync(useCase, null, cancellationToken);

        [HttpGet("messages/{chatId}")]
        public async Task<IActionResult> ListMessages(
            [FromServices] IListChatMessagesUseCase useCase,
            [FromRoute] ObjectId chatId,
            CancellationToken cancellationToken)
            => await RunAsync(useCase, chatId, cancellationToken);
    }
}

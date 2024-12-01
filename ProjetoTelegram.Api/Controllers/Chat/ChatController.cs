using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.MessageDTOs;
using ProjetoTelegram.Application.Interfaces.ChatInterfaces;
using ProjetoTelegram.Application.UseCases.ChatUseCases;
using System.Security.Claims;

namespace ProjetoTelegram.Api.Controllers.Chat
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ChatController : BaseController
    {
        private readonly IChatService _chatService;

        // todo: refatorar todas as controllers
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        public async Task<IActionResult> List(
            [FromServices] IListUserChatsUseCase useCase,
            CancellationToken cancellationToken)
        {
            var result = await RunAsync(useCase, null, cancellationToken);

            return Ok(result.Value);
        }

        [HttpGet("messages/{chatId}")]
        public async Task<IActionResult> ListMessages(
            [FromServices] IListChatMessagesUseCase useCase,
            [FromRoute] ObjectId chatId,
            CancellationToken cancellationToken)
        {
            var result = await RunAsync(useCase, chatId, cancellationToken);

            return Ok(result.Value);
        }
    }
}

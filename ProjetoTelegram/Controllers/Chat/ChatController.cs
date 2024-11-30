using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjetoTelegram.Domain.Models.Chat;
using ProjetoTelegram.Domain.Services.ChatServices;
using System.Security.Claims;

namespace ProjetoTelegram.Api.Controllers.Chat
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            return Ok(await _chatService.GetAll(new ObjectId(userId)));
        }

        [HttpGet("messages/{chatId}")]
        public async Task<IActionResult> ListMessages([FromRoute] ObjectId chatId)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            return Ok(await _chatService.GetMessages(new ObjectId(userId), chatId));
        }

        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] NewMessageModel newMessageModel)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            return Ok(await _chatService.SendMessage(newMessageModel));
        }
    }
}

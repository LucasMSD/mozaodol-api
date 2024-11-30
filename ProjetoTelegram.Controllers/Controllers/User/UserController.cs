using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjetoTelegram.Domain.Models.ContactList;
using ProjetoTelegram.Domain.Models.User;
using ProjetoTelegram.Domain.Services.UserServices;
using System.Security.Claims;

namespace ProjetoTelegram.Controllers.Controllers.User
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> ListAllUsers()
        {
            return Ok(await _userService.GetAll());
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            return Ok(await _userService.Get(new ObjectId(userId)));
        }


        [HttpGet("contacts")]
        public async Task<IActionResult> ListContacts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(await _userService.GetContacts(new ObjectId(userId)));
        }

        [HttpPost("contacts/add")]
        public async Task<IActionResult> AddContact([FromBody] AddContactModel addContact)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(await _userService.AddContact(new ObjectId(userId), addContact));
        }

        [HttpDelete("contacts/remove/{contactId}")]
        public async Task<IActionResult> AddContact([FromRoute] ObjectId contactId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(await _userService.RemoveContact(new ObjectId(userId), contactId));
        }

        [HttpPut("pushToken")]
        public async Task<IActionResult> UpdatePushToken([FromBody] UpdatePushTokenModel updatePushTokenModel)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _userService.UpdatePushToken(new ObjectId(userId), updatePushTokenModel);
            return Ok(new { Message = "Atualizado" });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.ContactDTOs;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using ProjetoTelegram.Application.Interfaces.UserInterfaces;
using ProjetoTelegram.Application.UseCases.UserUseCases;
using System.Security.Claims;

namespace ProjetoTelegram.Api.Controllers.User
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent(
            [FromServices] IGetCurrentUserDtoUseCase useCase,
            CancellationToken cancellationToken)
        {
            var result = await RunAsync(useCase, null, cancellationToken);

            if (result.IsFailed) return NotFound(result);

            return Ok(result.Value);
        }


        [HttpPut("pushToken")]
        public async Task<IActionResult> UpdatePushToken(
            [FromServices] IUpdatePushTokenUseCase useCase,
            [FromBody] UpdatePushTokenDTO input,
            CancellationToken cancellationToken)
        {
            var result = await RunAsync(useCase, input, cancellationToken);

            return Ok(new { Message = "Atualizado" });
            
        }
    }
}

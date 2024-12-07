using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using ProjetoTelegram.Application.UseCases.UserUseCases;

namespace ProjetoTelegram.Api.Controllers.User
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController : BaseController
    {

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent(
            [FromServices] IGetCurrentUserDtoUseCase useCase,
            CancellationToken cancellationToken)
            => await RunAsync(useCase, null, cancellationToken);


        [HttpPut("pushToken")]
        public async Task<IActionResult> UpdatePushToken(
            [FromServices] IUpdatePushTokenUseCase useCase,
            [FromBody] UpdatePushTokenDTO input,
            CancellationToken cancellationToken)
            => await RunAsync(useCase, input, cancellationToken);
    }
}

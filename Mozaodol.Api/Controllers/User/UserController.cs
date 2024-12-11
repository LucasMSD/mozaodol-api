using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozaodol.Application.DTOs.UserDTOs;
using Mozaodol.Application.UseCases.UserUseCases;

namespace Mozaodol.Api.Controllers.User
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController : DefaultController
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

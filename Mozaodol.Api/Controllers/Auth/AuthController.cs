using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mozaodol.Application.DTOs.AuthDTOs;
using Mozaodol.Application.UseCases.Auth.AuthUseCases;

namespace Mozaodol.Api.Controllers.Auth
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class AuthController : BaseController
    {
        [HttpPost("signup")]
        public async Task<IActionResult> Signup(
            [FromServices] IAuthSignupUseCase useCase,
            [FromBody] AuthSignupModel input,
            CancellationToken cancellationToken)
            => await RunAsync(useCase, input, cancellationToken);

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromServices] IAuthLoginUseCase useCase,
            [FromBody] AuthLoginModel input,
            CancellationToken cancellationToken)
            => await RunAsync(useCase, input, cancellationToken);
    }
}

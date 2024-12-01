using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoTelegram.Application.DTOs.AuthDTOs;
using ProjetoTelegram.Application.UseCases.Auth.AuthUseCases;

namespace ProjetoTelegram.Api.Controllers.Auth
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {


        [HttpPost("signup")]
        public async Task<IActionResult> Signup(
            [FromServices] IAuthSignupUseCase useCase,
            [FromBody] AuthSignupModel signupModel,
            CancellationToken cancellationToken)
        {

            var result = await useCase.Handle(signupModel, cancellationToken);

            if (result.IsFailed)
            {
                return BadRequest($"Usuário {signupModel.Username} já existe");
            }

            return Ok(result.Value);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromServices] IAuthLoginUseCase useCase,
            [FromBody] AuthLoginModel authLoginModel,
            CancellationToken cancellationToken)
        {
            var result = await useCase.Handle(authLoginModel, cancellationToken);

            if (result.IsFailed)
            {
                return BadRequest($"Email ou login errados");
            }

            return Ok(new
            {
                Token = result.Value,
            });
        }
    }
}

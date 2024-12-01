using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoTelegram.Application.DTOs.AuthDTOs;
using ProjetoTelegram.Application.Interfaces.AuthInterfaces;

namespace ProjetoTelegram.Api.Controllers.Auth
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] AuthSignupModel signupModel)
        {
            var result = await _authService.Signup(signupModel);

            if (result.IsFailed)
            {
                return BadRequest($"Usuário {signupModel.Username} já existe");
            }

            return Ok(result.Value);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthLoginModel authLoginModel)
        {
            var result = await _authService.Login(authLoginModel);

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

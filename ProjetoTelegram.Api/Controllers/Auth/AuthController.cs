using Microsoft.AspNetCore.Mvc;
using ProjetoTelegram.Application.Interfaces.AuthInterfaces;
using ProjetoTelegram.Domain.Models.Auth;

namespace ProjetoTelegram.Api.Controllers.Auth
{
    [ApiController]
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

            if (result == null)
            {
                return BadRequest($"Usuário {signupModel.Username} já existe");
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthLoginModel authLoginModel)
        {
            var result = _authService.Login(authLoginModel);

            if (string.IsNullOrEmpty(result))
            {
                return BadRequest($"Email ou login errados");
            }

            return Ok(new
            {
                Token = result,
            });
        }
    }
}

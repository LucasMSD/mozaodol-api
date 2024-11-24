using Microsoft.AspNetCore.Mvc;
using ProjetoTelegram.Domain.Models.Auth;
using ProjetoTelegram.Domain.Services.AuthServices;

namespace ProjetoTelegram.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
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
